using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;

namespace CUSTIS.NetCore.Lightbox.Observers
{
    /// <summary> Observer of message forwarding </summary>
    public interface IForwardObserver
    {
        /// <summary> Called when exception occurred during while message forwarding </summary>
        Task ForwardFault(ILightboxMessage message, Exception exception, CancellationToken token);
    }
}