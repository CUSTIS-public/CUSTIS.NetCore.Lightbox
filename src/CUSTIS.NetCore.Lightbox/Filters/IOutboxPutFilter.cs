using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
    public interface IOutboxPutFilter
    {
        /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
        Task PutMessage(PutContext context, PutDelegate next, CancellationToken token);
    }
}