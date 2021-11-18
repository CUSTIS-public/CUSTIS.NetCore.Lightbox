using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DomainModel;
using Moq;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Mocks
{
    internal sealed class MessageRepoMock : MockSkeleton<ILightboxMessageRepository>
    {
        private readonly List<ILightboxMessage> _createdMessages = new List<ILightboxMessage>();

        public IReadOnlyCollection<ILightboxMessage> CreatedMessages => _createdMessages;

        public MessageRepoMock()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            _createdMessages.Clear();

            Mock.Setup(r => r.Create(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    (CancellationToken t) =>
                    {
                        var lightboxMessage = new LightboxMessage();
                        _createdMessages.Add(lightboxMessage);

                        return lightboxMessage;
                    });
        }

        public void SetupGetMessagesToProcess(ILightboxMessage message)
        {
            Mock.Setup(r => r.GetMessagesToForward(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReadOnlyCollection<ILightboxMessage>(new[] { message }));
        }

        public void AssertRemoveInvoked(ILightboxMessage message, Func<Times> times)
        {
            Mock.Verify(r => r.Remove(message), times);
        }

        public void AssertSaveChangesAsyncInvoked(Func<Times> times)
        {
            Mock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), times);
        }
    }
}