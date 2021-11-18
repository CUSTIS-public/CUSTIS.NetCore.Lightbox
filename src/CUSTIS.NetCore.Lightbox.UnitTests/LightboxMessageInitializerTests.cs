using System.Collections.Generic;
using System.ServiceModel.Channels;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Sending;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests
{
    public class LightboxMessageInitializerTests
    {
        [Test]
        public void CreateMessage_SimpleCase_PropertiesFilledCorrect()
        {
            //Arrange
            var messageCreator = new LightboxMessageInitializer();
            var headers = new Dictionary<string, string> { { "h", "v" } };
            PutContext putContext = new("type", new object(), "serialized", headers);
            var message = new LightboxMessage();

            //Act
            var outboxMessage = messageCreator.FillMessage(message, putContext);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxMessage.MessageType, Is.EqualTo(putContext.MessageType));
                    Assert.That(outboxMessage.Body, Is.EqualTo(putContext.SerializedBody));
                    Assert.That(outboxMessage.Headers, Is.EqualTo(JsonConvert.SerializeObject(headers)));
                    Assert.That(outboxMessage.State, Is.EqualTo(LightboxMessageState.Created));
                    Assert.That(outboxMessage.Error, Is.Null.Or.Empty);
                    Assert.That(outboxMessage.AttemptCount, Is.EqualTo(0));
                });
        }
    }
}