using CUSTIS.NetCore.Lightbox.DAL;
using CUSTIS.NetCore.Lightbox.Utils;
using Microsoft.Practices.Unity;
using Moq;

namespace CUSTIS.NetCore.Lightbox.Unity.UnitTests.Common
{
    public class ContainerBuilder
    {
        public static UnityContainer PrepareContainer()
        {
            var container = new UnityContainer();
            container.AddSingleton(Mock.Of<ILightboxMessageRepository>());
            container.AddSingleton(Mock.Of<ISerializer>());

            return container;
        }
    }
}