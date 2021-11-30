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
        /// <param name="batchCount">Кол-во сообщений, которые требуется получить</param>
        /// <param name="maxAttemptsCount">Максимально допустимое значение поля <see cref="ILightboxMessage.AttemptCount"/></param>
        /// <param name="moduleName">Имя модуля (для случая, когда на одной БД работает несколько модулей)</param>
        /// <param name="token">Токен отмены</param>
        Task<IReadOnlyCollection<ILightboxMessage>> GetMessagesToForward(int batchCount, long maxAttemptsCount, string? moduleName, CancellationToken token);

        /// <summary> Удалить сообщение </summary>
        Task Remove(ILightboxMessage message);

        /// <summary> Пробрасывает изменения в БД</summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}