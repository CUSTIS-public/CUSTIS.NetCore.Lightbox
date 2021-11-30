using System.Linq;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Utils;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Заполняет поля сообщения Outbox </summary>
    internal class LightboxMessageInitializer : ILightboxMessageInitializer
    {
        private readonly ExtendedJsonConvert _jsonConvert;

        private readonly ILightboxOptions _options;

        /// <summary> Заполняет поля сообщения Outbox </summary>
        public LightboxMessageInitializer(ExtendedJsonConvert jsonConvert, ILightboxOptions options)
        {
            _jsonConvert = jsonConvert;
            _options = options;
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
            message.ModuleName = _options.ModuleName;

            if (context.Headers.Any())
            {
                message.Headers = _jsonConvert.Serialize(context.Headers);
            }

            return message;
        }
    }
}