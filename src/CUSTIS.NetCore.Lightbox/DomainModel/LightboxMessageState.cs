namespace CUSTIS.NetCore.Lightbox.DomainModel
{
    /// <summary> Состояние сообщения </summary>
    public enum LightboxMessageState
    {
        /// <summary> Создано </summary>
        Created,

        /// <summary> При обработке произошла ошибка </summary>
        Error,

        /// <summary> Отправлено </summary>
        Sent,

        /// <summary> Лимит попыток исчерпан </summary>
        DeadLetter = 4
    }
}