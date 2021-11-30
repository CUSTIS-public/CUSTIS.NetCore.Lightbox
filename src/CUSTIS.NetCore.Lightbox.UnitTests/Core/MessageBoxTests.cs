using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApprovalUtilities.Utilities;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Sending;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using CUSTIS.NetCore.Lightbox.UnitTests.Mocks;
using CUSTIS.NetCore.Lightbox.UnitTests.TestServices;
using CUSTIS.NetCore.Lightbox.Utils;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
{
    public class MessageBoxTests
    {
        private const string MessageType = "TestMSG";

        private static readonly MessageRepoMock Repo = new();

        private readonly MessageBox _box = CreateMessageBox();

        [SetUp]
        public void SetUp()
        {
            Repo.Reset();
        }

        #region Put

        [Test]
        public async Task Put_NoDto_CreateMessage()
        {
            //Arrange

            //Act
            await _box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    Assert.That(Repo.CreatedMessages.First().MessageType, Is.EqualTo(MessageType));
                });
        }

        [Test]
        public async Task Put_HasDto_CreatesMessage()
        {
            //Arrange

            //Act
            await _box.Put(MessageType, new Dto("Msg"));

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    var message = Repo.CreatedMessages.First();

                    Assert.That(message.Body, Is.Not.Null);
                });
        }

        #endregion

        #region Фильтры

        private class PutFilter : IOutboxPutFilter
        {
            public long InvocationCount { get; private set; }

            public string Name { get; set; } = nameof(PutFilter);

            /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
            public Task PutMessage(PutContext context, PutDelegate next, CancellationToken token)
            {
                InvocationCount++;

                context.Headers[Name] = (context.Headers.Count + 1).ToString();

                return next(context, token);
            }
        }

        [Test]
        public async Task Put_HasOneFilter_FilterInvoked()
        {
            //Arrange
            var putFilter = new PutFilter();
            var box = CreateMessageBox(putFilter);

            //Act
            await box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(putFilter.InvocationCount, Is.EqualTo(1));
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    Assert.That(Repo.CreatedMessages.First().MessageType, Is.EqualTo(MessageType));
                });
        }

        private static MessageBox CreateMessageBox(params IOutboxPutFilter[] putFilters)
        {
            var jsonConvert = new ExtendedJsonConvert(new OutboxJsonConvert());

            var initializer = new LightboxMessageInitializer(jsonConvert, Mock.Of<ILightboxOptions>());

            return new MessageBox(Repo.Object, putFilters, initializer, jsonConvert);
        }

        [Test]
        public async Task Put_HasManyFilters_FiltersInvoked()
        {
            //Arrange
            var putFilter = new PutFilter();
            var putFilter2 = new PutFilter();
            var box = CreateMessageBox(putFilter, putFilter2);

            //Act
            await box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(putFilter.InvocationCount, Is.EqualTo(1));
                    Assert.That(putFilter2.InvocationCount, Is.EqualTo(1));
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    Assert.That(Repo.CreatedMessages.First().MessageType, Is.EqualTo(MessageType));
                });
        }

        [Test]
        public async Task Put_HasManyFilters_CorrectOrder()
        {
            //Arrange
            var putFilter = new PutFilter { Name = "F1" };
            var putFilter2 = new PutFilter { Name = "F2" };
            var box = CreateMessageBox(putFilter, putFilter2);

            //Act
            await box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    var headers = JsonConvert.DeserializeObject<IReadOnlyDictionary<string, string>>(
                        Repo.CreatedMessages.First().Headers!);
                    Assert.That(
                        headers, Is.EquivalentTo(new Dictionary<string, string> { { "F1", "1" }, { "F2", "2" } }));
                });
        }

        #endregion

        #region Заголовки

        private class HeaderFilter : IOutboxPutFilter
        {
            private readonly IReadOnlyDictionary<string, string> _headers;

            public HeaderFilter(IReadOnlyDictionary<string, string> headers)
            {
                _headers = headers;
            }

            /// <summary> Фильтр, вызываемый во время добавления сообщения в Outbox </summary>
            public Task PutMessage(PutContext context, PutDelegate next, CancellationToken token)
            {
                context.Headers.AddAll(_headers);

                return next(context, token);
            }
        }

        [Test]
        public async Task Put_NoHeaders_NullHeadersSaved()
        {
            //Arrange
            var box = CreateMessageBox();

            //Act
            await box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    Assert.That(Repo.CreatedMessages.First().Headers, Is.Null);
                });
        }

        [Test]
        public async Task Put_HeadersSet_HeadersSaved()
        {
            //Arrange
            var initialHeaders = new Dictionary<string, string> { { "key", "value" }, { "k", "v" } };
            var filter = new HeaderFilter(initialHeaders);
            var box = CreateMessageBox(filter);

            //Act
            await box.Put(MessageType);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(Repo.CreatedMessages, Has.Exactly(1).Items);
                    var headers = Repo.CreatedMessages.First().Headers;
                    Assert.That(headers, Is.Not.Null);
                    var dictionary = JsonConvert.DeserializeObject<IReadOnlyDictionary<string, string>>(headers!);
                    Assert.That(dictionary, Is.EquivalentTo(initialHeaders));
                });
        }

        #endregion
    }
}