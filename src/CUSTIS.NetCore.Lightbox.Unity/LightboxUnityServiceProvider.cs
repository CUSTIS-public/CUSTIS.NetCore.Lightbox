using System;
using CUSTIS.NetCore.Lightbox.DependencyInjection;
using Microsoft.Practices.Unity;

namespace CUSTIS.NetCore.Lightbox.Unity
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
    }
}