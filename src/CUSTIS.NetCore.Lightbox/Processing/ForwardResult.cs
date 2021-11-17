namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <summary> Результат перенаправления сообщений Outbox в конечные системы </summary>
    public class ForwardResult
    {
        /// <summary> Результат перенаправления сообщений Outbox в конечные системы </summary>
        public ForwardResult(long successCount, long errorsCount)
        {
            SuccessCount = successCount;
            ErrorsCount = errorsCount;
        }

        /// <summary> Кол-во успешно перенаправленных сообщений </summary>
        public long SuccessCount { get; }

        /// <summary> Кол-во сообщений, при перенаправлении которых произошли ошибки </summary>
        public long ErrorsCount { get; }
    }
}