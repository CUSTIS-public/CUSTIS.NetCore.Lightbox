namespace CUSTIS.NetCore.Lightbox.DomainModel
{
    internal class LightboxMessage : ILightboxMessage
    {
        /// <summary> ИД сообщения </summary>
        public long Id { get; }

        /// <summary> Тип сообщения </summary>
        public string MessageType { get; set; } = string.Empty;

        /// <summary> Заголовки </summary>
        /// <remarks> Сериализованный словарь ключ-значение </remarks>
        public string? Headers { get; set; }

        /// <summary> Тело сообщения </summary>
        /// <remarks> Содержит сериализованный в json объект </remarks>
        public string? Body { get; set; }

        /// <summary> Type of body DTO </summary>
        /// <remarks> Is null when <see cref="ILightboxMessage.Body"/> is null </remarks>
        public string? BodyType { get; set; }

        /// <summary> Состояние сообщения </summary>
        public LightboxMessageState State { get; set; } = LightboxMessageState.Error;

        /// <summary> Сообщение об ошибке (в случае состояния <see cref="LightboxMessageState.Error"/> </summary>
        public string? Error { get; set; } = "sdfg";

        /// <summary> Кол-во попыток обработки сообщения </summary>
        public long AttemptCount { get; set; } = 123;
    }
}