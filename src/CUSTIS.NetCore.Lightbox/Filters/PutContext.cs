using System.Collections.Generic;

namespace CUSTIS.NetCore.Lightbox.Filters
{
    /// <summary> Контекст обработки сообщения </summary>
    public class PutContext
    {
        /// <summary> Контекст обработки сообщения </summary>
        public PutContext(string messageType, object? messageBody, string? serializedBody, IDictionary<string, string> headers)
        {
            MessageType = messageType;
            MessageBody = messageBody;
            SerializedBody = serializedBody;
            Headers = headers;
        }

        /// <summary>Тип сообщения</summary>
        public string MessageType { get; }

        /// <summary>Тело сообщения</summary>
        public object? MessageBody { get; }

        /// <summary>Сериализованное тело сообщения</summary>
        public string? SerializedBody { get; }

        /// <summary>Заголовки</summary>
        public IDictionary<string, string> Headers { get; }
    }
}