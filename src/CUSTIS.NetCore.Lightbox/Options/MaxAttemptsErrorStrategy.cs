namespace CUSTIS.NetCore.Lightbox.Options
{
    /// <summary> Стратегия обработки ошибочных сообщений, по которым достигнут лимит обработки </summary>
    public enum MaxAttemptsErrorStrategy
    {
        /// <summary> Оставить сообщение (и больше его не обрабатывать) </summary>
        Retain,

        /// <summary> Удалить сообщение </summary>
        Delete
    }
}