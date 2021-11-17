namespace CUSTIS.NetCore.Lightbox.DomainModel
{
    /// <summary> Состояние сообщения </summary>
    internal enum OutboxMessageState
    {
        /// <summary> Создано </summary>
        Created,

        /// <summary> При обработке произошла ошибка </summary>
        Error
    }
}