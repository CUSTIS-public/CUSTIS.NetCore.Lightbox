using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    internal class LightboxMsdiServiceProvider : ILightboxServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public LightboxMsdiServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary> Получить обязательный сервис </summary>
        public object GetRequiredService(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }
    }
}