using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
{
    public class SwitchmanTypeTests
    {
        [Test]
        public void Ctor_CorrectSwitchman_SwitchmanTypeCreated()
        {
            //Arrange

            //Act
            var switchmanType = new SwitchmanType(typeof(TestSwitchman), typeof(TestSwitchman));

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmanType.RegisteredType, Is.EqualTo(typeof(TestSwitchman)));
                    Assert.That(switchmanType.ImplementationType, Is.EqualTo(typeof(TestSwitchman)));
                });
        }

        [Test]
        public void Ctor_SwitchmanDoesNotImplementISwitchman_ExceptionThrown()
        {
            //Arrange

            //Act & Assert
            Assert.That(
                () => new SwitchmanType(typeof(string), typeof(string)),
                Throws.InvalidOperationException.With.Message.EqualTo("System.String не реализует ISwitchman"));
        }

        [Test]
        public void Ctor_ImplementationTypeDoesNotImplementRegisteredType_ExceptionThrown()
        {
            //Arrange

            //Act & Assert
            Assert.That(
                () => new SwitchmanType(typeof(string), typeof(TestSwitchman)),
                Throws.InvalidOperationException.With.Message.EqualTo("CUSTIS.NetCore.Lightbox.UnitTests.Common.TestSwitchman не реализует System.String"));
        }
    }
}