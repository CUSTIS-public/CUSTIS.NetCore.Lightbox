using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Lightbox.Sending
{
    /// <summary> Ящик для отправки сообщений Outbox </summary>
    public class MessageBox : IMessageBox
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;

        private readonly IReadOnlyCollection<IOutboxPutFilter> _putFilters;

        /// <summary> Отправитель сообщений </summary>
        public MessageBox(IOutboxMessageRepository outboxMessageRepository, IEnumerable<IOutboxPutFilter> putFilters)
        {
            _outboxMessageRepository = outboxMessageRepository;
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
            var message = new OutboxMessage(context.MessageType);

            message.Body = context.SerializedBody;

            if (context.Headers.Any())
            {
                message.Headers = JsonConvert.SerializeObject(context.Headers);
            }

            await _outboxMessageRepository.Create(message, token);
        }
    }
}