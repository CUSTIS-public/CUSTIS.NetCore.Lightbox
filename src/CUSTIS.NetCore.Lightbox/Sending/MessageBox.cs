using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.Filters;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Ящик для отправки сообщений Outbox </summary>
    public class MessageBox : IMessageBox
    {
        private readonly ILightboxMessageRepository _lightboxMessageRepository;

        private readonly ILightboxMessageInitializer _messageInitializer;

        private readonly IReadOnlyCollection<IOutboxPutFilter> _putFilters;

        /// <summary> Отправитель сообщений </summary>
        public MessageBox(ILightboxMessageRepository lightboxMessageRepository, IEnumerable<IOutboxPutFilter> putFilters,
                          ILightboxMessageInitializer messageInitializer)
        {
            _lightboxMessageRepository = lightboxMessageRepository;
            _messageInitializer = messageInitializer;
            _putFilters = putFilters.Reverse().ToArray();
        }

        /// <summary> Положить сообщение в Outbox </summary>
        public async Task Put(string messageType, object? dto = null, CancellationToken token = default)
        {
            PutDelegate putDelegate = CreateMessage;

            foreach (var filter in _putFilters)
            {
                var localDelegate = putDelegate;
                putDelegate = (context, tok) => filter.PutMessage(context, localDelegate, tok);
            }

            string? serializedBody = null;
            if (dto is { })
            {
                serializedBody = JsonConvert.SerializeObject(dto);
            }

            var putContext = new PutContext(messageType, dto, serializedBody, new Dictionary<string, string>());
            await putDelegate(putContext, token);
        }

        private async Task CreateMessage(PutContext context, CancellationToken token)
        {
            var message = await _lightboxMessageRepository.Create(token);
            _messageInitializer.FillMessage(message, context);
        }
    }
}