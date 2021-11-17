using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DomainModel;
using Moq;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Mocks
{
    internal sealed class MessageRepoMock : MockSkeleton<IOutboxMessageRepository>
    {
        private readonly List<OutboxMessage> _createdMessages = new List<OutboxMessage>();

        public IReadOnlyCollection<OutboxMessage> CreatedMessages => _createdMessages;

        public MessageRepoMock()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            _createdMessages.Clear();

            Mock.Setup(r => r.Create(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()))
                .Callback<OutboxMessage, CancellationToken>((m, t) => _createdMessages.Add(m));
        }

        public void SetupGetMessagesToProcess(OutboxMessage message)
        {
            Mock.Setup(r => r.GetMessagesToForward(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<OutboxMessage>(new[] { message }));
        }

        public void AssertRemoveInvoked(OutboxMessage message, Func<Times> times)
        {
            Mock.Verify(r => r.Remove(message), times);
        }

        public void AssertSaveChangesAsyncInvoked(Func<Times> times)
        {
            Mock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), times);
        }
    }
}