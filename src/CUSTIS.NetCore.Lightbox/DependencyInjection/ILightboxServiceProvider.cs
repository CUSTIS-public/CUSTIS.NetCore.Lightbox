using System;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    /// <summary> DI-контейнер, из которого Lightbox получает необходимые сервисы </summary>
    internal interface ILightboxServiceProvider
    {
        /// <summary> Получить обязательный сервис </summary>
        object GetRequiredService(Type type);
    }
}