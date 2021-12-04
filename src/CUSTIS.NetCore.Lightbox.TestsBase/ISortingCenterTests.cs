using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.TestsBase
{
    /// <summary> Тесты на <see cref="ISortingCenter"/> </summary>
    public interface ISortingCenterTests
    {
        #region Успешная обработка

        /// <summary> Успешно обработанное сообщение должно удаляться из БД </summary>
        Task ForwardMessages_SuccessProcessing_MessageIsDeleted();

        /// <summary> Сообщение без DTO должно передавать в стрелочника </summary>
        Task ForwardMessages_HasMessageWithoutDto_MessageProcessedBySwitchman();

        /// <summary> Сообщение с DTO должно передавать в стрелочника </summary>
        Task ForwardMessages_HasMessageWithDto_MessageProcessedBySwitchman();

        #endregion

        #region Параллельная обработка

        /// <summary> При параллельной обработке каждое сообщение обрабатывается только 1 раз </summary>
        Task ForwardMessages_ParallelExecution_EachMessageIsHandledOnce();

        #endregion

        #region Обработка ошибочных сообщений

        /// <summary> После коммита блокировка с ошибочного сообщения должна быть снята </summary>
        Task ForwardMessages_ExceptionMessage_LockIsReleased();

        /// <summary> Ошибочное сообщение должно быть удалено из БД после успешной обработки </summary>
        Task ForwardMessages_ExceptionMessage_MessageIsDeletedAfterSuccess();

        /// <summary> При превышении максимального числа попыток сообщение должно быть удалено из БД (если выбрана стратегия <see cref="MaxAttemptsErrorStrategy.Delete"/>) </summary>
        Task ForwardMessages_ExceptionMessageAfterMaxAttemptsAndDeleteStrategy_MessageIsDeleted();

        /// <summary> При превышении максимального числа попыток сообщение должно остаться в БД (если выбрана стратегия <see cref="MaxAttemptsErrorStrategy.Retain"/>) </summary>
        Task ForwardMessages_ExceptionMessageAfterMaxAttemptsAndRetainStrategy_MessageIsRetained();

        #endregion
    }
}