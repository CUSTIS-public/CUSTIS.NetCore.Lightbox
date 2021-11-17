using System;

namespace CUSTIS.NetCore.Lightbox.DomainModel
{
    /// <summary> Сообщение </summary>
    internal class OutboxMessage
    {
        /// <summary> Сообщение </summary>
        public OutboxMessage(string messageType)
        {
            MessageType = messageType;
        }

        /// <summary>Идентификатор</summary>
        public long Id { get; set; }

        /// <summary>Когда создан</summary>
        //TODO SMDISP-8993 заполнение
        public DateTime CreationDateTime { get; set; }

        /// <summary>Когда изменен</summary>
        //TODO SMDISP-8993 заполнение
        public DateTime ModificationDateTime { get; set; }

        /// <summary> Тип сообщения </summary>
        public string MessageType { get; set; }

        /// <summary> Заголовки </summary>
        /// <remarks> Сериализованный словарь ключ-значение </remarks>
        public string? Headers { get; set; }

        /// <summary> Тело сообщения </summary>
        /// <remarks> Содержит сериализованный в json объект </remarks>
        public string? Body { get; set; }

        /// <summary> Состояние сообщения </summary>
        public OutboxMessageState State { get; set; } = OutboxMessageState.Created;

        /// <summary> Сообщение об ошибке (в случае состояния <see cref="OutboxMessageState.Error"/> </summary>
        public string? Error { get; set; }

        /// <summary> Кол-во попыток обработки сообщения </summary>
        public long AttemptCount { get; set; }
    }
}