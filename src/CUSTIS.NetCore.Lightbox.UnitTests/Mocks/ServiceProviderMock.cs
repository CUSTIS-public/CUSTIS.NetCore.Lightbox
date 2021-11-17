using System;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Mocks
{
    /// <summary> Мок для <see cref="IServiceProvider"/> </summary>
    public class ServiceProviderMock : MockSkeleton<ILightboxServiceProvider>
    {
        /// <summary> Сбросить состояние мока </summary>
        public override void Reset()
        {
            base.Reset();

            Mock.Setup(m => m.CreateScope()).Returns(Moq.Mock.Of<ILightboxContainerScope>(
                                                         s => s.ServiceProvider == Object));
        }

        /// <summary> Настроить метод <see cref="IServiceProvider.GetService"/> </summary>
        public void SetupGetService<T>(T service) where T : class
        {
            Mock.Setup(s => s.GetRequiredService(typeof(T)))
                .Returns(service);
        }

        /// <summary> Настроить метод <see cref="ServiceProviderServiceExtensions.GetServices"/> </summary>
        public void SetupGetServices<T>(params T[] services) where T : class
        {
            Mock.Setup(s => s.GetServices<T>()).Returns(services);
        }
    }
}