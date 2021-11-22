using System;
using System.Collections.Generic;
using System.Linq;
using CUSTIS.NetCore.Lightbox.Processing;
using Microsoft.Practices.Unity;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    internal static class OutboxUnityExtensions
    {
        public static IEnumerable<SwitchmanType> EnumerateSwitchmans(this IUnityContainer container)
        {
            foreach (var registration in container.Registrations.Where(IsSwitchman))
            {
                yield return new SwitchmanType(registration.RegisteredType, registration.MappedToType);
            }
        }

        private static bool IsSwitchman(ContainerRegistration s)
        {
            var switchmanType = typeof(ISwitchman);
            return switchmanType.IsAssignableFrom(s.MappedToType);
        }
    }
}