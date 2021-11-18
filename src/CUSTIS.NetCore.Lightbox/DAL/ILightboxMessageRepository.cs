using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;

namespace CUSTIS.NetCore.Lightbox.DAL
{
    /// <summary> Репозиторий сообщений Outbox </summary>
    public interface ILightboxMessageRepository
    {
        /// <summary> Создать сообщение </summary>
        Task<ILightboxMessage> Create(CancellationToken token);

        /// <summary> Получить сообщения для обработки </summary>
        Task<IReadOnlyCollection<ILightboxMessage>> GetMessagesToForward(int batchCount, CancellationToken token);

        /// <summary> Удалить сообщение </summary>
        void Remove(ILightboxMessage message);

        /// <summary> Пробрасывает изменения в БД</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}