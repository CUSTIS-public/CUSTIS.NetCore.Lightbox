using System;
using System.Collections.Generic;
using System.Linq;
using CUSTIS.NetCore.Lightbox.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    internal static class OutboxMsdiExtensions
    {
        public static IEnumerable<SwitchmanType> EnumerateSwitchmans(this IServiceCollection serviceCollection)
        {
            foreach (var serviceDescriptor in serviceCollection.Where(IsSwitchman))
            {
                yield return new SwitchmanType(serviceDescriptor.ServiceType, GetImplementationType(serviceDescriptor));
            }
        }

        private static bool IsSwitchman(ServiceDescriptor s)
        {
            var switchmanType = typeof(ISwitchman);
            return switchmanType.IsAssignableFrom(s.ServiceType)
                   || switchmanType.IsAssignableFrom(s.ImplementationType)
                   || switchmanType.IsAssignableFrom(s.ImplementationInstance?.GetType());
        }

        private static Type GetImplementationType(ServiceDescriptor serviceDescriptor)
        {
            return serviceDescriptor.ImplementationType
                   ?? serviceDescriptor.ImplementationInstance?.GetType()
                   ?? throw new NotImplementedException(
                       "Стрелочники, заданные через ImplementationFactory, не поддерживаются");
        }
    }
}