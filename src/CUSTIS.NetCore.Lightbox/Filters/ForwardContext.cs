using System.Collections.Generic;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Контекст обработки сообщения </summary>
    public class ForwardContext
    {
        /// <summary> Контекст обработки сообщения </summary>
        public ForwardContext(long id, string messageType, long attemptCount, object? messageBody, string? serializedBody, IReadOnlyDictionary<string, string> headers)
        {
            Id = id;
            MessageType = messageType;
            AttemptCount = attemptCount;
            MessageBody = messageBody;
            SerializedBody = serializedBody;
            Headers = headers;
        }

        /// <summary>ИД сообщения</summary>
        public long Id { get; }

        /// <summary>Тип сообщения</summary>
        public string MessageType { get; }

        /// <summary>Попытка обработки</summary>
        public long AttemptCount { get; }

        /// <summary>Десериализованное тело сообщения</summary>
        public object? MessageBody { get; }

        /// <summary>Сериализованное тело сообщения</summary>
        public string? SerializedBody { get; }

        /// <summary>Заголовки</summary>
        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}