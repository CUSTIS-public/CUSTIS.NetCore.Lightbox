using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Outbox.Contracts;
using CUSTIS.NetCore.Outbox.Contracts.Filters;
using CUSTIS.NetCore.Outbox.DomainModel;
using CUSTIS.NetCore.Outbox.Processing;
using CUSTIS.NetCore.Tests.Mocks;
using CUSTIS.NetCore.UnitTests.Outbox.Builders;
using CUSTIS.NetCore.UnitTests.Outbox.Common;
using CUSTIS.NetCore.UnitTests.Outbox.Mocks;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.UnitTests.Outbox
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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var outboxOptions = Mock.Of<IOutboxOptions>(o => o.MaxAttemptsCount == MaxAttempts);
            _sortingCenter = CreateSortingCenter(outboxOptions);
        }

        private SortingCenter CreateSortingCenter(IOutboxOptions outboxOptions)
        {
            return new(
                _messageRepo.Object, _switchmanCollection.Object,
                _serviceProvider.Object, outboxOptions);
        }

        [SetUp]
        public void SetUp()
        {
            _messageRepo.Reset();
            _switchmanCollection.Reset();
            _switchman.Reset();
            _serviceProvider.Reset();
            _serviceProvider.SetupGetServices<IOutboxForwardFilter>();
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
            _messageRepo.SetupGetMessagesToProcess(message);

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
            _messageRepo.SetupGetMessagesToProcess(message);
            var options = Mock.Of<IOutboxOptions>(
                o => o.MaxAttemptsCount == MaxAttempts
                     && o.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Delete);
            var sortingCenter = CreateSortingCenter(options);

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
            _messageRepo.SetupGetMessagesToProcess(message);
            var options = Mock.Of<IOutboxOptions>(
                o => o.MaxAttemptsCount == MaxAttempts
                     && o.MaxAttemptsErrorStrategy == MaxAttemptsErrorStrategy.Retain);
            var sortingCenter = CreateSortingCenter(options);

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
            _messageRepo.SetupGetMessagesToProcess(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            _messageRepo.AssertSaveChangesAsyncInvoked(Times.Once);
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(OutboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring(TestSwitchman.ErrMessage));
                });
        }

        [Test]
        public async Task ForwardMessages_ErrorMessage_AttemptCountIsIncremented()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ThrowError());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToProcess(message);
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
            _messageRepo.SetupGetMessagesToProcess(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(OutboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("Произошла ошибочка"));
                });
        }

        [Test]
        public async Task ForwardMessages_AsyncTaskOfTSubscriber_ResultIsAwaited()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessAsyncWithResult());
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToProcess(message);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(message.State, Is.EqualTo(OutboxMessageState.Error));
                    Assert.That(message.Error, Contains.Substring("Произошла ошибочка"));
                });
        }

        [Test]
        public async Task ForwardMessages_SubscriberHasCancellationToken_TokenIsProvided()
        {
            //Arrange
            _switchmanCollection.SetupGet<TestSwitchman>(s => s.ProcessWithToken(It.IsAny<CancellationToken>()));
            var message = new OutboxMessageBuilder().Build();
            _messageRepo.SetupGetMessagesToProcess(message);
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
            _messageRepo.SetupGetMessagesToProcess(message);
            using var tokenSource = new CancellationTokenSource();
            Assume.That(_switchman.Token, Is.Null);

            //Act
            await _sortingCenter.ForwardMessages(token: tokenSource.Token);

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(_switchman.Token, Is.EqualTo(tokenSource.Token));
                    Assert.That(_switchman.Dto?.Msg, Is.EqualTo(msg));
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
            _messageRepo.SetupGetMessagesToProcess(message);
            var testFilter = new TestFilter();
            _serviceProvider.SetupGetServices<IOutboxForwardFilter>(testFilter);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await _sortingCenter.ForwardMessages();

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
            _messageRepo.SetupGetMessagesToProcess(message);
            var testFilter = new TestFilter();
            var testFilter2 = new TestFilter();
            _serviceProvider.SetupGetServices<IOutboxForwardFilter>(testFilter, testFilter2);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await _sortingCenter.ForwardMessages();

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
            _messageRepo.SetupGetMessagesToProcess(message);
            var testFilter = new TestFilter { Name = "F1" };
            var testFilter2 = new TestFilter { Name = "F2" };
            var order = testFilter.Order = testFilter2.Order = new Dictionary<string, long>();
            _serviceProvider.SetupGetServices<IOutboxForwardFilter>(testFilter, testFilter2);
            var testSwitchman = new TestSwitchman();
            _serviceProvider.SetupGetService(testSwitchman);

            //Act
            await _sortingCenter.ForwardMessages();

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
            _messageRepo.SetupGetMessagesToProcess(message);
            var filter = new HeadersFilter();
            _serviceProvider.SetupGetServices<IOutboxForwardFilter>(filter);

            //Act
            await _sortingCenter.ForwardMessages();

            //Assert
            Assert.That(filter.Headers, Is.EquivalentTo(initialHeaders));
        }

        #endregion
    }
}