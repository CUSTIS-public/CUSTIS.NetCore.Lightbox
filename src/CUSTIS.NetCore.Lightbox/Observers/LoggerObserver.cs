using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;
using Microsoft.Extensions.Logging;

namespace CUSTIS.NetCore.Lightbox.Observers
{
    /// <summary> Журналирует пересылку сообщений </summary>
    public class LoggerObserver : IForwardObserver
    {
        private readonly ILogger<LoggerObserver> _logger;

        /// <summary> Журналирует пересылку сообщений </summary>
        public LoggerObserver(ILogger<LoggerObserver> logger)
        {
            _logger = logger;
        }

        /// <summary> Called when exception occurred during while message forwarding </summary>
        public Task ForwardFault(ILightboxMessage message, Exception exception, CancellationToken token)
        {
            _logger.LogError(
                exception,
                "При обработке сообщения {MessageId} с типом {MessageType}, попытка {AttemptCount} произошла ошибка [{Type}] [{Message}]",
                message.Id, message.MessageType, message.AttemptCount, exception.GetType().Name, exception.Message);

            return Task.CompletedTask;
        }

        /// <summary> Вызывается, если требуется удаление сообщения при достижении максимальной попытки обработки </summary>
        public Task DeleteDueToMaxAttempts(ILightboxMessage message, CancellationToken token)
        {
            _logger.LogInformation(
                "Удаляем сообщение {MessageId} с типом {MessageType}: достигли макс. кол-ва попыток {AttemptCount}",
                message.Id, message.MessageType, message.AttemptCount);

            return Task.CompletedTask;
        }
    }
}