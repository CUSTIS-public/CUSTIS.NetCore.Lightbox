using System;
using System.Collections.Generic;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Sending;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
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
                    Assert.That(outboxMessage.BodyType, Is.EqualTo(putContext.MessageBody!.GetType().AssemblyQualifiedName));
                    Assert.That(outboxMessage.Headers, Is.EqualTo(JsonConvert.SerializeObject(headers)));
                    Assert.That(outboxMessage.State, Is.EqualTo(LightboxMessageState.Created));
                    Assert.That(outboxMessage.Error, Is.Null.Or.Empty);
                    Assert.That(outboxMessage.AttemptCount, Is.EqualTo(0));
                });
        }

        [Test]
        public void CreateMessage_SimpleCase_BodyTypeCanBeObtained()
        {
            //Arrange
            var messageCreator = new LightboxMessageInitializer();
            var headers = new Dictionary<string, string>();
            var messageBody = new Dto("msg");
            PutContext putContext = new("type", messageBody, "serialized", headers);
            var message = new LightboxMessage();

            //Act
            var outboxMessage = messageCreator.FillMessage(message, putContext);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(outboxMessage.BodyType, Is.EqualTo(putContext.MessageBody!.GetType().AssemblyQualifiedName));
                    Assert.That(Type.GetType(outboxMessage.BodyType!), Is.EqualTo(messageBody.GetType()));
                });
        }
    }
}