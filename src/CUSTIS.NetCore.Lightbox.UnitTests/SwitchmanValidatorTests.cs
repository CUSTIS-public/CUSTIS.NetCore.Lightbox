using System;
using System.Threading.Tasks;
using CUSTIS.NetCore.Outbox.Contracts;
using CUSTIS.NetCore.Outbox.Processing;
using NUnit.Framework;

namespace CUSTIS.NetCore.UnitTests.Outbox
{
    public class SwitchmanValidatorTests
    {
        private class SomeSwitchman
        {
            public async void ReturnVoidFromAsync()
            {
                await Task.CompletedTask;
            }

            public async Task ReturnTaskFromAsync()
            {
                await Task.CompletedTask;
            }

            public async Task<long> ReturnTaskOfTFromAsync()
            {
                await Task.CompletedTask;

                return 10;
            }
        }

        [Test]
        public void ThrowIfNecessary_AsyncMethodReturnVoid_Throws()
        {
            //Arrange
            var validator = new SwitchmanValidator();

            //Act & assert
            var result = validator.Validate(
                typeof(SomeSwitchman).GetMethod(nameof(SomeSwitchman.ReturnVoidFromAsync))!,
                new SwitchmanAttribute("type"));

            Assert.Multiple(
                () =>
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => validator.ThrowIfNecessary());
                    Assert.That(result, Is.False);
                    Assert.That(
                        exception?.Message,
                        Is.EqualTo(
                            $"Асинхронные методы стрелочников должны возвращать Task. Ошибочные методы: [{nameof(SomeSwitchman)}.{nameof(SomeSwitchman.ReturnVoidFromAsync)}]"));
                });
        }

        [Test]
        [TestCase(nameof(SomeSwitchman.ReturnTaskFromAsync))]
        [TestCase(nameof(SomeSwitchman.ReturnTaskOfTFromAsync))]
        public void ThrowIfNecessary_AsyncMethodReturnsTask_Throws(string methodName)
        {
            //Arrange
            var validator = new SwitchmanValidator();

            //Act & assert
            var result = validator.Validate(typeof(SomeSwitchman).GetMethod(methodName)!, new SwitchmanAttribute("type"));
            Assert.Multiple(
                () =>
                {
                    Assert.DoesNotThrow(() => validator.ThrowIfNecessary());
                    Assert.That(result, Is.True);
                });
        }
    }
}