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
        /// <remarks> Создать сообщение без добавления в сессию NH / контекст EF </remarks>
        Task<ILightboxMessage> Create(CancellationToken token);

        /// <summary> Сохранить сообщение </summary>
        /// <remarks> Добить сообщение в сессию NH / контекст EF </remarks>
        Task Save(ILightboxMessage message, CancellationToken token);

        /// <summary> Получить сообщения для обработки </summary>
        Task<IReadOnlyCollection<ILightboxMessage>> GetMessagesToForward(int batchCount, CancellationToken token);

        /// <summary> Удалить сообщение </summary>
        Task Remove(ILightboxMessage message);

        /// <summary> Пробрасывает изменения в БД</summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}