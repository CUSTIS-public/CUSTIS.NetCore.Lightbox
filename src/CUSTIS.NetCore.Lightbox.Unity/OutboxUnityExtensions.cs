using System.Collections.Generic;
using System.Linq;
using CUSTIS.NetCore.Lightbox.Processing;
using Unity;

namespace CUSTIS.NetCore.Lightbox.Unity
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

        private static bool IsSwitchman(IContainerRegistration s)
        {
            var switchmanType = typeof(ISwitchman);
            return switchmanType.IsAssignableFrom(s.MappedToType);
        }
    }
}