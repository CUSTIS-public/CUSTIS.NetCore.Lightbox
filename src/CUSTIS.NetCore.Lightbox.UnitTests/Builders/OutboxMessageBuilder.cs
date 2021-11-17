using System.Collections.Generic;
using System.Linq;
using CUSTIS.NetCore.Outbox.DomainModel;
using CUSTIS.NetCore.UnitTests.Outbox.Common;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.UnitTests.Outbox.Builders
{
    internal class OutboxMessageBuilder
    {
        public OutboxMessage Build()
        {
            var message = new OutboxMessage(string.Empty)
            {
                AttemptCount = _attemptCount,
                Headers = JsonConvert.SerializeObject(_headers)
            };

            if (_dto != null)
            {
                message.Body = JsonConvert.SerializeObject(_dto);
            }

            return message;
        }

        private long _attemptCount = 0;

        public OutboxMessageBuilder WithAttemptCount(long attemptCount)
        {
            _attemptCount = attemptCount;

            return this;
        }

        private IDictionary<string, string> _headers = new Dictionary<string, string>();

        public OutboxMessageBuilder WithHeaders(IReadOnlyDictionary<string, string> headers)
        {
            _headers = headers.ToDictionary(s => s.Key, s => s.Value);

            return this;
        }

        private Dto? _dto = null;

        public OutboxMessageBuilder WitDto(Dto dto)
        {
            _dto = dto;

            return this;
        }
    }
}