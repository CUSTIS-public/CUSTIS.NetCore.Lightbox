using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Observers;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Utils;

namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <inheritdoc />
    internal class SortingCenter : ISortingCenter
    {
        private const int DefaultBatchCount = 50;

        private readonly ILightboxMessageRepository _lightboxMessageRepository;

        private readonly ISwitchmanCollection _switchmans;

        private readonly ILightboxServiceProvider _serviceProvider;

        private readonly ILightboxOptions _lightboxOptions;

        private readonly ExtendedJsonConvert _jsonConvert;

        private readonly TypeLoader _typeLoader;

        private readonly IEnumerable<IForwardObserver> _forwardObservers;

        /// <summary> Обработчик сообщений </summary>
        public SortingCenter(
            ILightboxMessageRepository lightboxMessageRepository, ISwitchmanCollection switchmans,
            ILightboxServiceProvider serviceProvider, ILightboxOptions lightboxOptions,
            ExtendedJsonConvert jsonConvert, TypeLoader typeLoader,
            IEnumerable<IForwardObserver>? forwardObservers = null)
        {
            _lightboxMessageRepository = lightboxMessageRepository;
            _switchmans = switchmans;
            _serviceProvider = serviceProvider;
            _lightboxOptions = lightboxOptions;
            _jsonConvert = jsonConvert;
            _typeLoader = typeLoader;
            _forwardObservers = forwardObservers ?? Array.Empty<IForwardObserver>();
        }

        /// <summary> Перенаправить сообщения Outbox в системы-получатели </summary>
        public async Task<ForwardResult> ForwardMessages(int? batchCount = null, CancellationToken token = default)
        {
            var messages = await _lightboxMessageRepository.GetMessagesToForward(batchCount ?? DefaultBatchCount, token);
            var success = 0;
            var errors = 0;

            foreach (var message in messages)
            {
                try
                {
                    await ForwardMessage(message, token);
                    await _lightboxMessageRepository.Remove(message);
                    success++;
                }
                catch (Exception e)
                {
                    foreach (var forwardObserver in _forwardObservers)
                    {
                        await forwardObserver.ForwardFault(message, e, token);
                    }

                    errors++;
                    message.State = LightboxMessageState.Error;
                    message.Error = e.ToString();
                    message.AttemptCount++;

                    if (_lightboxOptions.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Delete
                        && message.AttemptCount > _lightboxOptions.MaxAttemptsCount)
                    {
                        foreach (var forwardObserver in _forwardObservers)
                        {
                            await forwardObserver.DeleteDueToMaxAttempts(message, token);
                        }

                        await _lightboxMessageRepository.Remove(message);
                    }
                }
            }

            await _lightboxMessageRepository.SaveChangesAsync(token);

            return new ForwardResult(success, errors);
        }

        private async Task ForwardMessage(ILightboxMessage message, CancellationToken token)
        {
            var switchmanInfo = _switchmans.Get(message.MessageType);

            if (message.Body != null && string.IsNullOrEmpty(message.BodyType))
            {
                throw new InvalidOperationException($"Сообщение {message.Id} имеет тело, но не указан тип тела");
            }

            object? msgBody = null;
            if (message.Body != null)
            {
                var bodyType = _typeLoader.RetrieveType(message.BodyType!);
                msgBody = _jsonConvert.Deserialize(message.Body, bodyType);
            }

            var headers = message.Headers != null
                              ? _jsonConvert.Deserialize<IReadOnlyDictionary<string, string>>(message.Headers)
                              : new Dictionary<string, string>();
            var context = new ForwardContext(message.Id, message.MessageType, message.AttemptCount,
                                                    msgBody, message.Body, headers);

            var parameters = GetParameters(switchmanInfo, context, token);

            var subscriber = _serviceProvider.GetRequiredService(switchmanInfo.SwitchmanType);

            var messageFilters = _serviceProvider.GetServices<IOutboxForwardFilter>().Reverse();

            ForwardDelegate forwardDelegate = (x, y) => InvokeSwitchman(switchmanInfo, subscriber, parameters);

            foreach (var messageFilter in messageFilters)
            {
                var localDelegate = forwardDelegate;
                forwardDelegate = (ctx, t) => messageFilter.ForwardMessage(ctx, localDelegate, t);
            }

            await forwardDelegate(context, token);
        }

        private static List<object?>? GetParameters(
            SwitchmanInfo subscriberInfo, ForwardContext context, CancellationToken token)
        {
            var parameterInfos = subscriberInfo.MethodInfo.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return null;
            }

            var parameters = new List<object?>(parameterInfos.Length);

            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType == typeof(CancellationToken))
                {
                    parameters.Add(token);
                }
                else if (parameterInfo.ParameterType == typeof(ForwardContext))
                {
                    parameters.Add(context);
                }
                else if (parameterInfo.ParameterType.IsAssignableFrom(context.MessageBody?.GetType()))
                {
                    parameters.Add(context.MessageBody);
                }
                else
                {
                    throw new InvalidOperationException($"Недопустимый тип параметра: {parameterInfo.ParameterType}. Допустимы параметры типа {context.MessageBody?.GetType()}, {nameof(ForwardContext)}, {nameof(CancellationToken)}");
                }
            }

            return parameters;
        }

        private static async Task InvokeSwitchman(
            SwitchmanInfo switchmanInfo, object switchman, List<object?>? parameters)
        {
            var result = switchmanInfo.MethodInfo.Invoke(switchman, parameters?.ToArray());

            if (result is Task task)
            {
                await task;
            }
        }
    }
}