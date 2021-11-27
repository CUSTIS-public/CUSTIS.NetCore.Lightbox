using System.Linq;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Utils;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Заполняет поля сообщения Outbox </summary>
    internal class LightboxMessageInitializer : ILightboxMessageInitializer
    {
        private readonly ExtendedJsonConvert _jsonConvert;

        /// <summary> Заполняет поля сообщения Outbox </summary>
        public LightboxMessageInitializer(ExtendedJsonConvert jsonConvert)
        {
            _jsonConvert = jsonConvert;
        }

        /// <summary> Заполняет поля сообщения Outbox </summary>
        public ILightboxMessage FillMessage(ILightboxMessage message, PutContext context)
        {
            message.MessageType = context.MessageType;
            message.Body = context.SerializedBody;
            message.BodyType = context.MessageBody?.GetType().FullName;
            message.State = LightboxMessageState.Created;
            message.AttemptCount = 0;
            message.Error = null;

            if (context.Headers.Any())
            {
                message.Headers = _jsonConvert.Serialize(context.Headers);
            }

            return message;
        }
    }
}