using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Outbox.DomainModel;

namespace CUSTIS.NetCore.Outbox.DAL
{
    /// <summary> Репозиторий сообщений Outbox </summary>
    internal interface IOutboxMessageRepository
    {
        /// <summary> Получить сообщения для обработки </summary>
        Task<IReadOnlyCollection<OutboxMessage>> GetMessagesToForward(int batchCount, CancellationToken token);

        /// <summary> Удалить сообщение </summary>
        void Remove(OutboxMessage message);

        /// <summary> Пробрасывает изменения в БД</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task Create(OutboxMessage message, CancellationToken token);
    }
}