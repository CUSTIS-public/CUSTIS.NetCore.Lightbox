using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;

namespace CUSTIS.NetCore.Lightbox.Observers
{
    /// <summary> Наблюдает за пересылкой сообщений </summary>
    public interface IForwardObserver
    {
        /// <summary> Вызывается, если при пересылке возникло исключение </summary>
        Task ForwardFault(ILightboxMessage message, Exception exception, CancellationToken token);

        /// <summary> Вызывается, если требуется удаление сообщения при достижении максимальной попытки обработки </summary>
        Task DeleteDueToMaxAttempts(ILightboxMessage message, CancellationToken token);
    }
}