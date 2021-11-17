using System;

namespace CUSTIS.NetCore.Outbox.DependencyInjection
{
    /// <summary> Дочерний контейнер </summary>
    public interface ILightboxContainerScope : IDisposable
    {
        /// <summary> DI-контейнер, из которого Lightbox получает необходимые сервисы </summary>
        ILightboxServiceProvider ServiceProvider { get; }
    }
}