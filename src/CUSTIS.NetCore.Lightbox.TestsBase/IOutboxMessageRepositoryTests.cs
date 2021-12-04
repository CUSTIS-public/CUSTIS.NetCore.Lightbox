using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DAL;

namespace CUSTIS.NetCore.Lightbox.TestsBase
{
    /// <summary> Тесты на <see cref="ILightboxMessageRepository"/> </summary>
    public interface IOutboxMessageRepositoryTests
    {
        #region Порядок сообщений

        /// <summary> Вначале должны быть получены сообщения с минимальным числом попыток обработки </summary>
        Task GetMessagesToForward_ManyMessages_MessagesWithMinimumAttemptFirst();

        /// <summary> Если есть несколько сообщений с одинаковым кол-вом попыток обработки,
        /// то вначале обрабатываем те сообщения, которые были раньше созданы </summary>
        Task GetMessagesToForward_ManyMessages_MessagesAreOrderedByTimeStamp();

        #endregion

        #region Фильтры

        /// <summary> Необходимо получить не более <paramref name="batchCount"/> сообщений </summary>
        Task GetMessagesToForward_ManyMessages_BatchCountRetrieved(int batchCount);

        /// <summary> Сообщения, по которым превышено число попыток, не обрабатываем </summary>
        Task GetMessagesToForward_MaxAttemptsReached_MessageNotReturned();

        /// <summary> Получаем сообщения, относящиеся только к нашему модулю </summary>
        Task GetMessagesToForward_ModuleSet_OnlyCurrentModuleMessagesReturned();

        #endregion
    }
}