using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Observers;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests
{
    public class UnityValidatorTests
    {
        private readonly UnityValidator _unityValidator = new UnityValidator();

        [Test]
        public void Validate_PutFilterWithoutName_ExceptionThrown()
        {
            //Arrange
            var container = new UnityContainer();
            container.RegisterType<IOutboxPutFilter, TestPutFilter>();

            //Act & Assert
            Assert.That(() => _unityValidator.Validate(container),
                        Throws.Exception.With.Message.EqualTo("IOutboxPutFilter необходимо регистрировать через метод LightboxConfiguration.AddPutFilter"));
        }

        [Test]
        public void Validate_ForwardFilterWithoutName_ExceptionThrown()
        {
            //Arrange
            var container = new UnityContainer();
            container.RegisterType<IOutboxForwardFilter, TestForwardFilter>();

            //Act & Assert
            Assert.That(() => _unityValidator.Validate(container),
                        Throws.Exception.With.Message.EqualTo("IOutboxForwardFilter необходимо регистрировать через метод LightboxConfiguration.AddForwardFilter"));
        }

        [Test]
        public void Validate_ForwardObserverWithoutName_ExceptionThrown()
        {
            //Arrange
            var container = new UnityContainer();
            container.RegisterType<IForwardObserver, TestForwardObserver>();

            //Act & Assert
            Assert.That(() => _unityValidator.Validate(container),
                        Throws.Exception.With.Message.EqualTo("IForwardObserver необходимо регистрировать через метод LightboxConfiguration.AddForwardObserver"));
        }
    }
}