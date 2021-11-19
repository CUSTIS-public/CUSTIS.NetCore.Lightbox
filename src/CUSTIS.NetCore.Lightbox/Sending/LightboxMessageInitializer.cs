using System.Linq;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Initializes lightbox messages </summary>
    internal class LightboxMessageInitializer : ILightboxMessageInitializer
    {
        /// <summary> Initialize lightbox message according to data in <paramref name="context"/> </summary>
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
                message.Headers = JsonConvert.SerializeObject(context.Headers);
            }

            return message;
        }
    }
}