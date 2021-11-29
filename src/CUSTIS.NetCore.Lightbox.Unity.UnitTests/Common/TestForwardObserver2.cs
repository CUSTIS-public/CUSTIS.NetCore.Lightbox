using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Observers;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    public class TestForwardObserver2 : IForwardObserver
    {
        /// <summary> Вызывается, если при пересылке возникло исключение </summary>
        public Task ForwardFault(ILightboxMessage message, Exception exception, CancellationToken token)
        {
            return null;
        }

        /// <summary> Вызывается, если требуется удаление сообщения при достижении максимальной попытки обработки </summary>
        public Task DeleteDueToMaxAttempts(ILightboxMessage message, CancellationToken token)
        {
            return null;
        }
    }
}