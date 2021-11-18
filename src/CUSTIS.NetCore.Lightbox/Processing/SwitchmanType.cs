using System;

namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <summary> Информация о типе стрелочника </summary>
    public class SwitchmanType
    {
        /// <summary> Информация о типе стрелочника </summary>
        public SwitchmanType(Type registeredType, Type implementationType)
        {
            if (!typeof(ISwitchman).IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException($"{implementationType} не реализует {nameof(ISwitchman)}");
            }

            if (!registeredType.IsAssignableFrom(implementationType))
            {
                throw new InvalidOperationException($"{implementationType} не реализует {registeredType}");
            }

            RegisteredType = registeredType;
            ImplementationType = implementationType;
        }

        /// <summary> Тип, зарегистрированный в контейнере </summary>
        public Type RegisteredType { get; }

        /// <summary> Фактический тип стрелочника </summary>
        public Type ImplementationType { get; }
    }
}