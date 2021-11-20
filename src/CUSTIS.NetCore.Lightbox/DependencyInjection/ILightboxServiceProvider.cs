using System;
using System.Collections.Generic;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    /// <summary> DI-контейнер, из которого Lightbox получает необходимые сервисы </summary>
    internal interface ILightboxServiceProvider
    {
        /// <summary> Получить обязательный сервис </summary>
        object GetRequiredService(Type type);

        /// <summary> Получить сервисы </summary>
        IEnumerable<T> GetServices<T>();
    }
}