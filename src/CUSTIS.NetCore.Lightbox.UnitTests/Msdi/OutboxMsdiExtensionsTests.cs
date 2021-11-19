using System.Linq;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using CUSTIS.NetCore.UnitTests.Outbox.Common;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Msdi
{
    public class OutboxMsdiExtensionsTests
    {
        [Test]
        public void EnumerateSwitchmans_ScopedSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<TestSwitchman>();

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmans, Has.Exactly(1).Items);
                    Assert.That(switchmans[0].RegisteredType, Is.EqualTo(typeof(TestSwitchman)));
                    Assert.That(switchmans[0].ImplementationType, Is.EqualTo(typeof(TestSwitchman)));
                });
        }

        [Test]
        public void EnumerateSwitchmans_SingletonSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new TestSwitchman());

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmans, Has.Exactly(1).Items);
                    Assert.That(switchmans[0].RegisteredType, Is.EqualTo(typeof(TestSwitchman)));
                    Assert.That(switchmans[0].ImplementationType, Is.EqualTo(typeof(TestSwitchman)));
                });
        }

        [Test]
        public void EnumerateSwitchmans_SingletonWithInterfaceSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ISwitchman>(new TestSwitchman());

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmans, Has.Exactly(1).Items);
                    Assert.That(switchmans[0].RegisteredType, Is.EqualTo(typeof(ISwitchman)));
                    Assert.That(switchmans[0].ImplementationType, Is.EqualTo(typeof(TestSwitchman)));
                });
        }

        [Test]
        public void EnumerateSwitchmans_InterfaceWithoutISwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var message = "Тестируем случай, когда в контейнере лежит интерфейс, не реализующий ISwitchman (ISwitchman реализует конечный класс)";
            Assume.That(typeof(ITestSwitchman2), Is.Not.AssignableFrom(typeof(ISwitchman)), message);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestSwitchman2>(new TestSwitchman2());

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmans, Has.Exactly(1).Items);
                    Assert.That(switchmans[0].RegisteredType, Is.EqualTo(typeof(ITestSwitchman2)));
                    Assert.That(switchmans[0].ImplementationType, Is.EqualTo(typeof(TestSwitchman2)));
                });
        }

        [Test]
        public void EnumerateSwitchmans_Switchmans_EmptyCollection()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.That(switchmans, Has.Exactly(0).Items);
        }
    }
}