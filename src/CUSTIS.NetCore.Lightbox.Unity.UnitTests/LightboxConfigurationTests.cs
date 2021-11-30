using System.Linq;
using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Observers;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Sending;
using CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common;
using CUSTIS.NetCore.Lightbox.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests
{
    public class LightboxConfigurationTests
    {
        [SetUp]
        public void SetUp()
        {
            TestPutFilter.Reset();
            TestPutFilter2.Reset();
        }

        #region Проверяем достаточность регистраций зависимостей

        [Test]
        public void AddLightbox_MessageBoxObtained()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();

            //Act
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            var messageBox = container.Resolve<IMessageBox>();
            Assert.That(messageBox, Is.Not.Null);
        }

        [Test]
        public void AddLightbox_SortingCenterObtained()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();

            //Act
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Assert
            var sortingCenter = container.Resolve<ISortingCenter>();
            Assert.That(sortingCenter, Is.Not.Null);
        }

        #endregion

        #region Фильтры

        [Test]
        public void AddPutFilter_HasOnePutFilter_PutFilterInvoked()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Act
            container.AddPutFilter<TestPutFilter>(new ContainerControlledLifetimeManager());
            var messageBox = container.Resolve<IMessageBox>();
            messageBox.Put("type");

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(container.ResolveAll<IOutboxPutFilter>().Count(), Is.EqualTo(1));
                    Assert.That(TestPutFilter.PutContext, Is.Not.Null);
                });
        }

        [Test]
        public void AddPutFilter_HasManyPutFilters_PutFiltersInvoked()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Act
            container.AddPutFilter<TestPutFilter>(new ContainerControlledLifetimeManager());
            container.AddPutFilter<TestPutFilter2>(new ContainerControlledLifetimeManager());
            var messageBox = container.Resolve<IMessageBox>();
            messageBox.Put("type");

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(container.ResolveAll<IOutboxPutFilter>().Count(), Is.EqualTo(2));
                    Assert.That(TestPutFilter.PutContext, Is.Not.Null, "first");
                    Assert.That(TestPutFilter2.PutContext, Is.Not.Null, "second");
                });
        }

        [Test]
        public void AddForwardFilter_ForwardFiltersResolvedAsIEnumerable()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Act
            container.AddForwardFilter<TestForwardFilter>(new ContainerControlledLifetimeManager());
            container.AddForwardFilter<TestForwardFilter2>(new ContainerControlledLifetimeManager());

            //Assert
            Assert.That(container.ResolveAll<IOutboxForwardFilter>().Count(), Is.EqualTo(2));
        }

        [Test]
        public void AddForwardObserver_ForwardObserversResolvedAsIEnumerable()
        {
            //Arrange
            var container = ContainerBuilder.PrepareContainer();
            container.AddLightbox(Mock.Of<ILightboxOptions>());

            //Act
            container.AddForwardObserver<TestForwardObserver>(new ContainerControlledLifetimeManager());
            container.AddForwardObserver<TestForwardObserver2>(new ContainerControlledLifetimeManager());

            //Assert
            Assert.That(container.ResolveAll<IForwardObserver>().Count(), Is.EqualTo(2));
        }

        #endregion
    }
}