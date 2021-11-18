using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;

namespace CUSTIS.NetCore.Lightbox.DAL
{
    /// <summary> Репозиторий сообщений Outbox </summary>
    public interface IOutboxMessageRepository
    {
        /// <summary> Получить сообщения для обработки </summary>
        Task<IReadOnlyCollection<OutboxMessage>> GetMessagesToForward(int batchCount, CancellationToken token);

        /// <summary> Удалить сообщение </summary>
        void Remove(OutboxMessage message);

        /// <summary> Пробрасывает изменения в БД</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary> Создать сообщение </summary>
        Task Create(OutboxMessage message, CancellationToken token);
    }
}