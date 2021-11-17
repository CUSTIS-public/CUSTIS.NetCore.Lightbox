using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Фильтр, вызываемый во время пересылки сообщения из Outbox </summary>
    public interface IOutboxForwardFilter
    {
        /// <summary> Фильтр, вызываемый во время пересылки сообщения из Outbox </summary>
        Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token);
    }
}