using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Следующий по очереди обработчик сообщения </summary>
    public delegate Task PutDelegate(PutContext context, CancellationToken token);
}