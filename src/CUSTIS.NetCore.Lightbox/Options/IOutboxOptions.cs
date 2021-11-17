namespace CUSTIS.NetCore.Outbox.Contracts
{
    /// <summary> Опции для настройки Outbox </summary>
    public interface IOutboxOptions
    {
        /// <summary> Максимальное кол-во попыток обработки сообщения </summary>
        long MaxAttemptsCount { get; }

        /// <summary> Стратегия обработки ошибочных сообщений, по которым достигнут лимит обработки </summary>
        MaxAttemptsErrorStrategy MaxAttemptsErrorStrategy { get; set; }
    }
}