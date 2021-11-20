using System;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Mocks
{
    /// <summary> Мок для <see cref="IServiceProvider"/> </summary>
    internal class ServiceProviderMock : MockSkeleton<ILightboxServiceProvider>
    {
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