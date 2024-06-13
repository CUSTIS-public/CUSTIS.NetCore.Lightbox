using System;
using System.Linq;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Observers;
using Unity;

namespace CUSTIS.NetCore.Lightbox.Unity
{
    internal class UnityValidator
    {
        public void Validate(IUnityContainer container)
        {
            // Чтобы метод ResolveAll<T> отработал корректно, сервисы должны быть зарегистрированы под именем.
            // Это требование Unity
            var errorPutFilters = container.Registrations
                .Where(r => r.RegisteredType == typeof(IOutboxPutFilter) && string.IsNullOrEmpty(r.Name))
                .ToArray();
            if (errorPutFilters.Any())
            {
                throw new InvalidOperationException(
                    $"{nameof(IOutboxPutFilter)} необходимо регистрировать через метод {nameof(LightboxConfiguration)}.{nameof(LightboxConfiguration.AddPutFilter)}");
            }

            var errorForwardFilters = container.Registrations
                .Where(r => r.RegisteredType == typeof(IOutboxForwardFilter) && string.IsNullOrEmpty(r.Name))
                .ToArray();
            if (errorForwardFilters.Any())
            {
                throw new InvalidOperationException(
                    $"{nameof(IOutboxForwardFilter)} необходимо регистрировать через метод {nameof(LightboxConfiguration)}.{nameof(LightboxConfiguration.AddForwardFilter)}");
            }

            var errorForwardObservers = container.Registrations
                .Where(r => r.RegisteredType == typeof(IForwardObserver) && string.IsNullOrEmpty(r.Name))
                .ToArray();
            if (errorForwardObservers.Any())
            {
                throw new InvalidOperationException(
                    $"{nameof(IForwardObserver)} необходимо регистрировать через метод {nameof(LightboxConfiguration)}.{nameof(LightboxConfiguration.AddForwardObserver)}");
            }
        }
    }
}