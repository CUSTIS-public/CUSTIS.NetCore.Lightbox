using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CUSTIS.NetCore.Outbox.Processing;
using CUSTIS.NetCore.Tests.Mocks;
using Moq;

namespace CUSTIS.NetCore.UnitTests.Outbox.Mocks
{
    internal class SwitchmanCollectionMock : MockSkeleton<ISwitchmanCollection>
    {
        public void SetupGet<T>(Expression<Action<T>> expr)
        {
            Mock.Setup(m => m.Get(It.IsAny<string>()))
                .Returns(
                    new SwitchmanInfo(
                        typeof(T), string.Empty,
                        ((MethodCallExpression)expr.Body).Method));
        }

        public void SetupGet<T>(Expression<Func<T, Task>> expr)
        {
            Mock.Setup(m => m.Get(It.IsAny<string>()))
                .Returns(
                    new SwitchmanInfo(
                        typeof(T), string.Empty,
                        ((MethodCallExpression)expr.Body).Method));
        }

        public void SetupGet<T>(string methodName)
        {
            var method = typeof(T).GetMethod(methodName);

            if (method == null)
            {
                throw new NoNullAllowedException($"Не найден метод {methodName} в {typeof(T)}");
            }

            Mock.Setup(m => m.Get(It.IsAny<string>()))
                .Returns(new SwitchmanInfo(typeof(T), string.Empty, method));
        }
    }
}