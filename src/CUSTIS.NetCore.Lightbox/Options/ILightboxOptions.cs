namespace CUSTIS.NetCore.Lightbox.Options
{
    /// <summary> Опции для настройки Outbox </summary>
    public interface ILightboxOptions
    {
        /// <summary> Максимальное кол-во попыток обработки сообщения </summary>
        long MaxAttemptsCount { get; }

        /// <summary> Стратегия обработки ошибочных сообщений, по которым достигнут лимит обработки </summary>
        MaxAttemptsErrorStrategy MaxAttemptsErrorStrategy { get; }

        /// <summary> Имя модуля </summary>
        /// <remarks> Для случая, когда через одну БД работают несколько модулей </remarks>
        string? ModuleName { get; }
    }
}