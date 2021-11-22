using System.Linq;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests
{
    public class OutboxUnityExtensionsTests
    {
        [Test]
        public void EnumerateSwitchmans_ScopedSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var serviceCollection = new UnityContainer();
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
            var serviceCollection = new UnityContainer();
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
        public void EnumerateSwitchmans_InstanceByTypeRegistered_ReturnSwitchman()
        {
            //Arrange
            var serviceCollection = new UnityContainer();
            serviceCollection.AddSingleton<TestSwitchman>(new TestSwitchman());

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
        public void EnumerateSwitchmans_InterfaceWithoutISwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var message = "Тестируем случай, когда в контейнере лежит интерфейс, не реализующий ISwitchman (ISwitchman реализует конечный класс)";
            Assume.That(typeof(ITestSwitchman2), Is.Not.AssignableFrom(typeof(ISwitchman)), message);

            var serviceCollection = new UnityContainer();
            serviceCollection.AddSingleton<ITestSwitchman2, TestSwitchman2>();

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(switchmans, Has.Exactly(1).Items);
                    Assert.That(switchmans.ElementAtOrDefault(0)?.RegisteredType, Is.EqualTo(typeof(ITestSwitchman2)));
                    Assert.That(switchmans.ElementAtOrDefault(0)?.ImplementationType, Is.EqualTo(typeof(TestSwitchman2)));
                });
        }

        [Test]
        public void EnumerateSwitchmans_Switchmans_EmptyCollection()
        {
            //Arrange
            var serviceCollection = new UnityContainer();

            //Act
            var switchmans = serviceCollection.EnumerateSwitchmans().ToArray();

            //Assert
            Assert.That(switchmans, Has.Exactly(0).Items);
        }
    }
}