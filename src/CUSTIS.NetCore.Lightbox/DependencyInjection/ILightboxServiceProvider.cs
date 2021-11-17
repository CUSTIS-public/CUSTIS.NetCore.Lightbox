using System;
using System.Collections.Generic;

namespace CUSTIS.NetCore.Outbox.DependencyInjection
{
    /// <summary> DI-контейнер, из которого Lightbox получает необходимые сервисы </summary>
    public interface ILightboxServiceProvider
    {
        /// <summary> Создать дочерний контейнер </summary>
        ILightboxContainerScope CreateScope();

        /// <summary> Получить обязательный сервис </summary>
        object GetRequiredService(Type type);

        /// <summary> Получить сервисы </summary>
        IEnumerable<T> GetServices<T>();
    }
}