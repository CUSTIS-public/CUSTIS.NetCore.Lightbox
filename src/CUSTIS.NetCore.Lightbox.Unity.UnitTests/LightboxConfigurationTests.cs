using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Sending;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests
{
    public class LightboxConfigurationTests
    {
        [Test]
        public void AddLightbox_MessageBoxObtained()
        {
            //Arrange
            var container = PrepareContainer();

            //Act
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            var messageBox = container.Resolve<IMessageBox>();
            Assert.That(messageBox, Is.Not.Null);
        }

        private static UnityContainer PrepareContainer()
        {
            var container = new UnityContainer();
            container.AddSingleton<ILightboxMessageRepository>(Mock.Of<ILightboxMessageRepository>());

            return container;
        }

        [Test]
        public void AddLightbox_SortingCenterObtained()
        {
            //Arrange
            var container = PrepareContainer();

            //Act
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            var sortingCenter = container.Resolve<ISortingCenter>();
            Assert.That(sortingCenter, Is.Not.Null);
        }
    }
}