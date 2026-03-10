using System.Threading;
using System.Threading.Tasks;

namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <summary> Сортировочный центр Outbox </summary>
    public interface ISortingCenter
    {
        /// <summary> Перенаправить сообщения Outbox в системы-получатели </summary>
        Task<ForwardResult> ForwardMessages(int? batchCount = null, CancellationToken token = default);

        /// <summary> Удалить обработанные сообщения с истекшим интервалом хранения </summary>
        Task DeleteExpiredMessages(CancellationToken token = default);
    }
}