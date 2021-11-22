using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Sending;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Msdi
{
    public class LightboxConfigurationTests
    {
        [Test]
        public void AddLightbox_AllServicesRegistered()
        {
            //Arrange
            var serviceCollection = PrepareServices();

            //Act
            serviceCollection.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            serviceCollection.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }

        private static ServiceCollection PrepareServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILightboxMessageRepository>(Mock.Of<ILightboxMessageRepository>());

            return serviceCollection;
        }

        [Test]
        public void AddLightbox_MessageBoxObtained()
        {
            //Arrange
            var serviceCollection = PrepareServices();

            //Act
            serviceCollection.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            using var provider = serviceCollection.BuildServiceProvider();
            var messageBox = provider.GetRequiredService<IMessageBox>();
            Assert.That(messageBox, Is.Not.Null);
        }

        [Test]
        public void AddLightbox_SortingCenterObtained()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILightboxMessageRepository>(Mock.Of<ILightboxMessageRepository>());

            //Act
            serviceCollection.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            using var provider = serviceCollection.BuildServiceProvider();
            var sortingCenter = provider.GetRequiredService<ISortingCenter>();
            Assert.That(sortingCenter, Is.Not.Null);
        }
    }
}