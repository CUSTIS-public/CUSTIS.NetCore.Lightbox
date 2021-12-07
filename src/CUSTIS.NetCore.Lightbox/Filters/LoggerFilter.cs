using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Логирует отправку и обработку сообщений через Outbox </summary>
    public class LoggerFilter : IOutboxPutFilter, IOutboxForwardFilter
    {
        private readonly ILogger<LoggerFilter> _logger;

        /// <summary> Логирует отправку и обработку сообщений через Outbox </summary>
        public LoggerFilter(ILogger<LoggerFilter> logger)
        {
            _logger = logger;
        }

        /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
        public Task PutMessage(PutContext context, PutDelegate next, CancellationToken token)
        {
            _logger.LogInformation(
                "Добавляем в Outbox сообщение тип {MessageType}, заголовки [{Headers}], тело [{MessageBody}]",
                context.MessageType, context.Headers.Keys, context.SerializedBody);

            return next(context, token);
        }

        /// <summary> Фильтр, вызываемый во время пересылки сообщения из Outbox </summary>
        public Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token)
        {
            _logger.LogInformation(
                "Обрабатываем сообщение {MessageId} с типом {MessageType}, попытка {MessageAttempt}, заголовки [{Headers}], тело [{MessageBody}]",
                context.Id, context.MessageType, context.AttemptCount, context.Headers.Keys, context.SerializedBody);

            return next(context, token);
        }
    }
}