using CUSTIS.NetCore.Lightbox.Options;
using CUSTIS.NetCore.Lightbox.Processing;
using CUSTIS.NetCore.Lightbox.Sending;
using CUSTIS.NetCore.Lightbox.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace CUSTIS.NetCore.Lightbox.DependencyInjection
{
    /// <summary> Методы для конфигурирования приложения для работы с паттерном Outbox </summary>
    public static class LightboxConfiguration
    {
        /// <summary> Добавить сервисы, необходимые для работы Outbox </summary>
        /// <remarks> Перед вызовом <see cref="AddLightbox(Microsoft.Extensions.DependencyInjection.IServiceCollection,ILightboxOptions)"/> необходимо зарегистрировать
        /// всех <see cref="ISwitchman"/> в <paramref name="collection"/> </remarks>
        public static IServiceCollection AddLightbox(this IServiceCollection collection, ILightboxOptions outboxOptions)
        {
            collection.AddSingleton(outboxOptions);
            collection.AddSingleton<ISwitchmanCollection>(new SwitchmanCollection(collection.EnumerateSwitchmans()));
            collection.AddScoped<ISortingCenter, SortingCenter>();
            collection.AddScoped<ILightboxServiceProvider, LightboxMsdiServiceProvider>();
            collection.AddScoped<IMessageBox, MessageBox>();
            collection.AddSingleton<ILightboxMessageInitializer, LightboxMessageInitializer>();
            collection.AddSingleton<ExtendedJsonConvert>();

            return collection;
        }
    }
}