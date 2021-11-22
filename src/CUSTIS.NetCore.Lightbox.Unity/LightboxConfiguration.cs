using System.Collections.Generic;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Observers;
using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Sending;
using Microsoft.Practices.Unity;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    /// <summary> Методы для конфигурирования приложения для работы с паттерном Outbox </summary>
    public static class LightboxConfiguration
    {
        /// <summary> Добавить сервисы, необходимые для работы Outbox </summary>
        /// <remarks> Перед вызовом <see cref="AddLightbox(IUnityContainer,ILightboxOptions)"/> необходимо зарегистрировать
        /// всех <see cref="ISwitchman"/> в <paramref name="container"/> </remarks>
        public static IUnityContainer AddLightbox(this IUnityContainer container, ILightboxOptions outboxOptions)
        {
            container.AddSingleton(outboxOptions);
            container.AddSingleton<ISwitchmanCollection>(new SwitchmanCollection(container.EnumerateSwitchmans()));
            container.AddScoped<ISortingCenter, SortingCenter>();
            container.AddScoped<ILightboxServiceProvider, LightboxUnityServiceProvider>();
            container.AddScoped<IMessageBox, MessageBox>();
            container.AddSingleton<ILightboxMessageInitializer, LightboxMessageInitializer>();

            container.RegisterType<IEnumerable<IOutboxPutFilter>>(
                new InjectionFactory(c => c.ResolveAll<IOutboxPutFilter>()));
            container.RegisterType<IEnumerable<IForwardObserver>>(
                new InjectionFactory(c => c.ResolveAll<IForwardObserver>()));

            return container;
        }

        internal static void AddSingleton<T>(this IUnityContainer container)
        {
            container.RegisterType<T>(new ContainerControlledLifetimeManager());
        }

        internal static void AddSingleton<TI, TS>(this IUnityContainer container)
            where TS : TI
        {
            container.RegisterType<TI, TS>(new ContainerControlledLifetimeManager());
        }

        internal static void AddSingleton<TI>(this IUnityContainer container, TI service)
        {
            container.RegisterInstance<TI>(service, new ContainerControlledLifetimeManager());
        }

        internal static void AddScoped<T>(this IUnityContainer container)
        {
            container.RegisterType<T>(new HierarchicalLifetimeManager());
        }

        internal static void AddScoped<TI, TS>(this IUnityContainer container)
            where TS : TI
        {
            container.RegisterType<TI, TS>(new HierarchicalLifetimeManager());
        }
    }
}