using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.DomainModel;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.UnitTests.Builders;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using CUSTIS.NetCore.Lightbox.UnitTests.Mocks;
using CUSTIS.NetCore.Lightbox.UnitTests.TestServices;
using CUSTIS.NetCore.Lightbox.Utils;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
{
    public class SortingCenterTests
    {
        #region Инфраструктура

        private const long MaxAttempts = 100;

        private readonly MessageRepoMock _messageRepo = new();

        private readonly SwitchmanCollectionMock _switchmanCollection = new();

        private readonly ServiceProviderMock _serviceProvider = new();

        private readonly TestSwitchman _switchman = new();

        private SortingCenter _sortingCenter = default!;

        private Mock<ILightboxOptions> _outboxOptions = default!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _outboxOptions = new Mock<ILightboxOptions>();
            _outboxOptions.Setup(o => o.MaxAttemptsCount).Returns(MaxAttempts);
            _sortingCenter = CreateSortingCenter(_outboxOptions);
        }

        private SortingCenter CreateSortingCenter(Mock<ILightboxOptions> lightboxOptions, params IOutboxForwardFilter[] forwardFilters)
        {
            return new(
                _messageRepo.Object, _switchmanCollection.Object,
                _serviceProvider.Object, lightboxOptions.Object, new FriendlySerializer(new OutboxSerializer()),
                new TypeLoader(), forwardFilters);
        }

        [SetUp]
        public void SetUp()
        {
            _messageRepo.Reset();
            _switchmanCollection.Reset();
            _switchman.Reset();
            _serviceProvider.Reset();
            _serviceProvider.SetupGetService(_switchman);
        }

        #endregion

        #region Обработка успешных сообщений

        [Test]
        public async Task ForwardMessages_Success_MessageIsRemoved()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            _messageRepo.AssertRemoveInvoked(message, Times.Once);
        }

        #endregion

        #region Обработка ошибочных сообщений

        [Test]
        public async Task ForwardMessages_MaxAttemptsReachedAndDeleteStrategy_MessageIsRemoved()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ThrowError());
            var message = new OutboxMessageBuilder().WithAttemptCount(MaxAttempts).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var options = Mock.Of<ILightboxOptions>(
                o => o.MaxAttemptsCount == MaxAttempts
                     && o.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Delete);
            var sortingCenter = CreateSortingCenter(Mock.Get(options));

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            _messageRepo.AssertRemoveInvoked(message, Times.Once);
        }

        [Test]
        public async Task ForwardMessages_MaxAttemptsReachedAndRetainStrategy_MessageIsRetained()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ThrowError());
            var message = new OutboxMessageBuilder().WithAttemptCount(MaxAttempts).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var options = Mock.Of<ILightboxOptions>(
                o => o.MaxAttemptsCount == MaxAttempts
                     && o.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Retain);
            var sortingCenter = CreateSortingCenter(Mock.Get(options));

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            _messageRepo.AssertRemoveInvoked(message, Times.Never);
        }

        [Test]
        public async Task ForwardMessages_Exception_MessageStateIsError()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ThrowError());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(LightboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring(TestSwitchman.ErrMessage));
                });
        }

        [Test]
        public async Task ForwardMessages_ErrorMessage_AttemptCountIsIncremented()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ThrowError());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);
            Assume.That(message.AttemptCount, Is.EqualTo(0));

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.That(message.AttemptCount, Is.EqualTo(1));
        }

        #endregion

        #region Async-await

        [Test]
        public async Task ForwardMessages_AsyncSubscriber_ResultIsAwaited()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessAsync());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(LightboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("Произошла ошибочка"));
                });
        }

        [Test]
        public async Task ForwardMessages_AsyncTaskOfTSubscriber_ResultIsAwaited()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessAsyncWithResult());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(LightboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("Произошла ошибочка"));
                });
        }

        #endregion

        #region Фильтры сообщений

        private class TestFilter : IOutboxForwardFilter
        {
            public long InvocationCount { get; private set; }

            public string Name { get; set; } = nameof(TestFilter);

            public IDictionary<string, long>? Order { get; set; }

            /// <summary> Фильтр, вызываемый во время обработки сообщения </summary>
            public Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token)
            {
                InvocationCount++;

                if (Order is not null)
                {
                    Order[Name] = Order.Count + 1;
                }

                return next(context, token);
            }
        }

        [Test]
        public async Task ForwardMessages_OneFilter_FilterAndSwitchmanInvoked()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var testFilter = new TestFilter();
            var sortingCenter = CreateSortingCenter(_outboxOptions, testFilter);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(testFilter.InvocationCount, Is.EqualTo(1));
                    Assert.That(testSwitchman.InvocationCount, Is.EqualTo(1));
                });
        }

        [Test]
        public async Task ForwardMessages_MultipleFilters_FiltersAndSwitchmanInvoked()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var testFilter = new TestFilter();
            var testFilter2 = new TestFilter();
            var sortingCenter = CreateSortingCenter(_outboxOptions, testFilter, testFilter2);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(testFilter.InvocationCount, Is.EqualTo(1));
                    Assert.That(testFilter2.InvocationCount, Is.EqualTo(1));
                    Assert.That(testSwitchman.InvocationCount, Is.EqualTo(1));
                });
        }

        [Test]
        public async Task ForwardMessages_MultipleFilters_CorrectOrder()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var testFilter = new TestFilter { Name = "F1" };
            var testFilter2 = new TestFilter { Name = "F2" };
            var order = testFilter.Order = testFilter2.Order = new Dictionary<string, long>();
            var sortingCenter = CreateSortingCenter(_outboxOptions, testFilter, testFilter2);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(order[testFilter.Name], Is.EqualTo(1));
                    Assert.That(order[testFilter2.Name], Is.EqualTo(2));
                });
        }

        #endregion

        #region Заголовки

        private class HeadersFilter : IOutboxForwardFilter
        {
            public IReadOnlyDictionary<string, string>? Headers { get; private set; }

            /// <summary> Фильтр, вызываемый во время обработки сообщения </summary>
            public Task ForwardMessage(ForwardContext context, ForwardDelegate next, CancellationToken token)
            {
                Headers = context.Headers;

                return next(context, token);
            }
        }

        [Test]
        public async Task ForwardMessages_MessageWithHeaders_FilterHasHeaderInfo()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var initialHeaders = new Dictionary<string, string>() { { "k", "v" }, { "kk", "vv" } };
            var message = new OutboxMessageBuilder().WithHeaders(initialHeaders).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            var filter = new HeadersFilter();
            var sortingCenter = CreateSortingCenter(_outboxOptions, filter);

            //Act
            await sortingCenter.ForwardMessages();

            //Assert
            Assert.That(filter.Headers, Is.EquivalentTo(initialHeaders));
        }

        #endregion

        #region Тело сообщения

        [Test]
        public async Task ForwardMessages_BodyWithoutBodyType_MessageStateIsError()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().WitDto(new("My msg")).WithBodyType(null).Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            _messageRepo.AssertRemoveInvoked(message, Times.Never);
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(LightboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("System.InvalidOperationException: Сообщение 0 имеет тело, но не указан тип тела"));
                });
        }

        [Test]
        public async Task ForwardMessages_IllegalBodyType_MessageStateIsError()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessMessage());
            var message = new OutboxMessageBuilder().WitDto(new("My msg")).WithBodyType("Illegal").Build();
            _messageRepo.SetupGetMessagesToForward(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            _messageRepo.AssertRemoveInvoked(message, Times.Never);
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(LightboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("System.ArgumentException: Type Illegal doesn't exist in the current app domain"));
                });
        }

        #endregion

        #region Передача параметров в стрелочника

        [Test]
        public async Task ForwardMessages_SubscriberHasCancellationToken_TokenIsProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessWithToken(It.IsAny<CancellationToken>()));
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.That(_switchman.Token, Is.EqualTo(tokenSource.Token));
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasDtoAndCancellationToken_DtoAndTokenProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(nameof(TestSwitchman.ProcessDtoWithToken));
            var msg = "mymsg";
            var message = new OutboxMessageBuilder().WitDto(new Dto(msg)).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.Error, Is.Null.Or.Empty);
                    Assert.That(_switchman.Token, Is.EqualTo(tokenSource.Token));
                    Assert.That(_switchman.Dto?.Msg, Is.EqualTo(msg));
                });
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasDtoAndContextAndCancellationToken_DtoAndContextAndTokenProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(nameof(TestSwitchman.ProcessDtoContextToken));
            var msg = "mymsg";
            var message = new OutboxMessageBuilder().WitDto(new Dto(msg)).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);
            Assume.That(_switchman.Context, Is.Null);
            Assume.That(_switchman.Dto, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.Error, Is.Null.Or.Empty);
                    Assert.That(_switchman.Token, Is.EqualTo(tokenSource.Token));
                    Assert.That(_switchman.Dto?.Msg, Is.EqualTo(msg));
                    Assert.That(_switchman.Context, Is.Not.Null);
                    Assert.That(_switchman.Context?.MessageBody, Is.TypeOf<Dto>().With.Property(nameof(Dto.Msg)).EqualTo(msg));
                });
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasContextAndCancellationToken_ContextAndTokenProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(nameof(TestSwitchman.ProcessContextToken));
            var msg = "mymsg";
            var message = new OutboxMessageBuilder().WitDto(new Dto(msg)).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);
            Assume.That(_switchman.Context, Is.Null);
            Assume.That(_switchman.Dto, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.Error, Is.Null.Or.Empty);
                    Assert.That(_switchman.Token, Is.EqualTo(tokenSource.Token));
                    Assert.That(_switchman.Dto, Is.Null);
                    Assert.That(_switchman.Context, Is.Not.Null);
                    Assert.That(_switchman.Context?.MessageBody, Is.TypeOf<Dto>().With.Property(nameof(Dto.Msg)).EqualTo(msg));
                });
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasObjectParam_DtoProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(nameof(TestSwitchman.ProcessObject));
            var msg = "mymsg";
            var message = new OutboxMessageBuilder().WitDto(new Dto(msg)).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);
            Assume.That(_switchman.Context, Is.Null);
            Assume.That(_switchman.Dto, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.Error, Is.Null.Or.Empty);
                    Assert.That(_switchman.Dto?.Msg, Is.EqualTo(msg));
                });
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasIllegalParam_MessageError()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(nameof(TestSwitchman.ProcessIllegalParam));
            var msg = "mymsg";
            var message = new OutboxMessageBuilder().WitDto(new Dto(msg)).Build();
            _messageRepo.SetupGetMessagesToForward(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);
            Assume.That(_switchman.Context, Is.Null);
            Assume.That(_switchman.Dto, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.That(message.Error, Contains.Substring("System.InvalidOperationException: Недопустимый тип параметра: System.String"));
        }

        #endregion

        [Test]
        public async Task ForwardMessages_GetMessagesInvokedWithCorrectParams()
        {
            //Arrange
            using var tokenSource = new CancellationTokenSource();
            var moduleName = "name";
            var maxAttempts = 1;
            var batchCount = 2;
            _outboxOptions.Setup(o => o.ModuleName).Returns(moduleName);
            _outboxOptions.Setup(o => o.MaxAttemptsCount).Returns(maxAttempts);
            _messageRepo.SetupGetMessagesToForward(out var forwardParams);

            //Act
            await _sortingCenter.ForwardMessages(batchCount, tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(forwardParams.ModuleName, Is.EqualTo(moduleName));
                    Assert.That(forwardParams.MaxAttemptsCount, Is.EqualTo(maxAttempts));
                    Assert.That(forwardParams.BatchCount, Is.EqualTo(batchCount));
                });
        }
    }
}