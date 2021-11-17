using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Outbox.Contracts
{
    /// <summary> Ящик для отправки сообщений Outbox </summary>
    public interface IMessageBox
    {
        /// <summary> Положить сообщение в Outbox </summary>
        Task Put(string messageType, object? dto = null, CancellationToken token = default);
    }
}