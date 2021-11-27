using System.Collections.Generic;
using System.Linq;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Builders
{
    internal class OutboxMessageBuilder
    {
        public ILightboxMessage Build()
        {
            var message = new LightboxMessage
            {
                AttemptCount = _attemptCount,
                Headers = JsonConvert.SerializeObject(_headers),
                BodyType = _bodyType,
                Error = null
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
            _bodyType = dto.GetType().FullName;

            return this;
        }

        private string? _bodyType = null;

        public OutboxMessageBuilder WithBodyType(string? type)
        {
            _bodyType = type;

            return this;
        }
    }
}