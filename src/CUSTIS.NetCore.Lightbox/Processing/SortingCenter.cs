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
using Newtonsoft.Json;

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

        private readonly IForwardObserver? _forwardObserver;

        /// <summary> Обработчик сообщений </summary>
        public SortingCenter(
            ILightboxMessageRepository lightboxMessageRepository, ISwitchmanCollection switchmans,
            ILightboxServiceProvider serviceProvider, ILightboxOptions lightboxOptions,
            IForwardObserver? forwardObserver = null)
        {
            _lightboxMessageRepository = lightboxMessageRepository;
            _switchmans = switchmans;
            _serviceProvider = serviceProvider;
            _lightboxOptions = lightboxOptions;
            _forwardObserver = forwardObserver;
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
                    _lightboxMessageRepository.Remove(message);
                    success++;
                }
                catch (Exception e)
                {
                    if (_forwardObserver != null)
                    {
                        await _forwardObserver.ForwardFault(message, e, token);
                    }

                    errors++;
                    message.State = LightboxMessageState.Error;
                    message.Error = e.ToString();
                    message.AttemptCount++;

                    if (_lightboxOptions.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Delete
                        && message.AttemptCount > _lightboxOptions.MaxAttemptsCount)
                    {
                        _lightboxMessageRepository.Remove(message);
                    }
                }
            }

            await _lightboxMessageRepository.SaveChangesAsync(token);

            return new ForwardResult(success, errors);
        }

        private async Task ForwardMessage(ILightboxMessage message, CancellationToken token)
        {
            var subscriberInfo = _switchmans.Get(message.MessageType);

            var (parameters, msgBody) = GetParameters(subscriberInfo, message, token);
            var headers = message.Headers != null
                              ? ExtendedJsonConvert.Deserialize<IReadOnlyDictionary<string, string>>(message.Headers)
                              : new Dictionary<string, string>();
            var messageContext = new ForwardContext(message.Id, message.MessageType, message.AttemptCount,
                                                    msgBody, message.Body, headers);

            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var subscriber = serviceProvider.GetRequiredService(subscriberInfo.SwitchmanType);

            var messageFilters = serviceProvider.GetServices<IOutboxForwardFilter>().Reverse();

            ForwardDelegate forwardDelegate = (x, y) => InvokeSwitchman(subscriberInfo, subscriber, parameters);

            foreach (var messageFilter in messageFilters)
            {
                var localDelegate = forwardDelegate;
                forwardDelegate = (context, t) => messageFilter.ForwardMessage(context, localDelegate, t);
            }

            await forwardDelegate(messageContext, token);
        }

        private static (List<object?>? parameters, object? msgBody) GetParameters(
            SwitchmanInfo subscriberInfo, ILightboxMessage message, CancellationToken token)
        {
            var parameterInfos = subscriberInfo.MethodInfo.GetParameters();

            if (parameterInfos.Count(i => i.ParameterType != typeof(CancellationToken)) > 1)
            {
                throw new InvalidOperationException(
                    $"Метод {subscriberInfo.MethodInfo.GetMemberName()} содержит 2 или более dto-параметров");
            }

            if (parameterInfos.Length == 0)
            {
                return (null, null);
            }

            var parameters = new List<object?>(parameterInfos.Length);
            object? msgBody = null;

            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType == typeof(CancellationToken))
                {
                    parameters.Add(token);
                }
                else
                {
                    if (message.Body != null)
                    {
                        msgBody = JsonConvert.DeserializeObject(message.Body, parameterInfo.ParameterType);
                    }

                    parameters.Add(msgBody);
                }
            }

            return (parameters, msgBody);
        }

        private static async Task InvokeSwitchman(
            SwitchmanInfo subscriberInfo, object subscriber, List<object?>? parameters)
        {
            var result = subscriberInfo.MethodInfo.Invoke(subscriber, parameters?.ToArray());

            if (result is Task task)
            {
                await task;
            }
        }
    }
}