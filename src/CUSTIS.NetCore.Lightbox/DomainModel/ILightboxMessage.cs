namespace CUSTIS.NetCore.Lightbox.DomainModel
{
    /// <summary> Сообщение </summary>
    public interface ILightboxMessage
    {
        /// <summary> ИД сообщения </summary>
        public long Id { get; }

        /// <summary> Тип сообщения </summary>
        public string MessageType { get; set; }

        /// <summary> Заголовки </summary>
        /// <remarks> Сериализованный словарь ключ-значение </remarks>
        public string? Headers { get; set; }

        /// <summary> Тело сообщения </summary>
        /// <remarks> Содержит сериализованный в json объект </remarks>
        public string? Body { get; set; }

        /// <summary> Type of body DTO </summary>
        /// <remarks> Is null when <see cref="Body"/> is null </remarks>
        public string? BodyType { get; set; }

        /// <summary> Состояние сообщения </summary>
        public LightboxMessageState State { get; set; }

        /// <summary> Сообщение об ошибке (в случае состояния <see cref="LightboxMessageState.Error"/> </summary>
        public string? Error { get; set; }

        /// <summary> Кол-во попыток обработки сообщения </summary>
        public long AttemptCount { get; set; }
    }
}