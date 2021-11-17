using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Outbox.Contracts.Filters
{
    /// <summary> Следующий по очереди обработчик сообщения </summary>
    public delegate Task ForwardDelegate(ForwardContext context, CancellationToken token);
}