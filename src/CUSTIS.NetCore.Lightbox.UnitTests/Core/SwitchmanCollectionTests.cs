using System;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.UnitTests.Common;
using NUnit.Framework;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Core
{
    public class SwitchmanCollectionTests
    {
        [Test]
        public void Get_ScopeSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var switchmanType = new SwitchmanType(typeof(TestSwitchman), typeof(TestSwitchman));
            var switchmanCollection = new SwitchmanCollection(new[] { switchmanType });

            //Act
            var info = switchmanCollection.Get(nameof(TestSwitchman.ProcessMessage));

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(info, Is.Not.Null);
                    Assert.That(info.MessageType, Is.EqualTo(nameof(TestSwitchman.ProcessMessage)));
                    Assert.That(info.SwitchmanType, Is.EqualTo(typeof(TestSwitchman)));
                });
        }

        [Test]
        public void Get_SingletonWithInterfaceSwitchmanRegistered_ReturnsSwitchman()
        {
            //Arrange
            var switchmanType = new SwitchmanType(typeof(ISwitchman), typeof(TestSwitchman));
            var switchmanCollection = new SwitchmanCollection(new[] { switchmanType });

            //Act
            var info = switchmanCollection.Get(nameof(TestSwitchman.ProcessMessage));

            //Assert
            Assert.Multiple(
                () =>
                {
                    Assert.That(info, Is.Not.Null);
                    Assert.That(info.MessageType, Is.EqualTo(nameof(TestSwitchman.ProcessMessage)));
                    Assert.That(info.SwitchmanType, Is.EqualTo(typeof(ISwitchman)));
                });
        }

        [Test]
        public void Get_NoService_ExceptionIsThrown()
        {
            //Arrange
            var switchmanCollection = new SwitchmanCollection(Array.Empty<SwitchmanType>());

            //Act & Assert
            Assert.That(() => switchmanCollection.Get(nameof(TestSwitchman.ProcessMessage)),
                        Throws.Exception.With.Message.Contains(nameof(TestSwitchman.ProcessMessage)));
        }

        [Test]
        public void Ctor_AmbiguousRegistration_ExceptionIsThrown()
        {
            //Arrange
            var switchmanType = new SwitchmanType(typeof(ISwitchman), typeof(TestSwitchman));
            var switchmanType2 = new SwitchmanType(typeof(ISwitchman), typeof(SameMessageSwitchman));

            //Act & Assert
            Assert.That(
                () => new SwitchmanCollection(new[] { switchmanType, switchmanType2, }),
                Throws.Exception.With.Message.EqualTo(
                    $"Допустима регистрация только одного стрелочника для каждого типа сообщения. Для следующих сообщений зарегистрировано более одного стрелочника: {Environment.NewLine}" +
                    $"[ProcessMessage] - [TestSwitchman.ProcessMessage, SameMessageSwitchman.ProcessMessage]"));
        }

        [Test]
        public void Ctor_SwitchmanWithManyParams_ExceptionIsThrown()
        {
            //Arrange
            var switchmanType = new SwitchmanType(typeof(ISwitchman), typeof(MultiArgSwitchman));

            //Act & Assert
            Assert.That(
                () => new SwitchmanCollection(new[] { switchmanType, }),
                Throws.Exception.With.Message.EqualTo(
                    "В методах-стрелочниках допустимо использовать не более 1 dto-аргумента. Ошибочные методы: [MultiArgSwitchman.ProcessTwoArgs]"));
        }

        [Test]
        public void Ctor_SwitchmanWithDtoContextTokenArgs_CollectionCreated()
        {
            //Arrange
            var switchmanType = new SwitchmanType(typeof(ISwitchman), typeof(DtoContextTokenSwitchman));

            //Act
            var collection = new SwitchmanCollection(new[] { switchmanType, });

            //Assert
            Assert.Pass("Конструктор не должен бросить исключения");
        }
    }
}