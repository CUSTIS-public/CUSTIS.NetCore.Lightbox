using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    internal class LightboxMsdiScope : ILightboxContainerScope
    {
        private readonly IServiceScope _serviceScope;

        public LightboxMsdiScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        /// <summary> DI-контейнер, из которого Lightbox получает необходимые сервисы </summary>
        public ILightboxServiceProvider ServiceProvider =>
            new LightboxMsdiServiceProvider(_serviceScope.ServiceProvider);
    }
}