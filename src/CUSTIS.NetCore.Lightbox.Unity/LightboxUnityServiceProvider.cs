using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    internal class LightboxUnityServiceProvider : ILightboxServiceProvider
    {
        private readonly IUnityContainer _unityContainer;

        public LightboxUnityServiceProvider(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        /// <summary> Получить обязательный сервис </summary>
        public object GetRequiredService(Type type)
        {
            var service = _unityContainer.Resolve(type);

            if (service == null)
            {
                throw new InvalidOperationException($"В контейнере не зарегистрирован тип {type}");
            }

            return service;
        }

        /// <summary> Получить сервисы </summary>
        public IEnumerable<T> GetServices<T>()
        {
            return _unityContainer.ResolveAll<T>();
        }
    }
}