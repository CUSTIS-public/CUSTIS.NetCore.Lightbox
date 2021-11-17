using System;

namespace CUSTIS.NetCore.Lightbox.Processing
{
    /// <summary> Информация о типе стрелочника </summary>
    public class SwitchmanType
    {
        /// <summary> Информация о типе стрелочника </summary>
        public SwitchmanType(Type registeredType, Type implementationType)
        {
            // TODO SMDISP-8993 Тест на то, что либо один, либо другой тип должны имплементить ISwitchman
            RegisteredType = registeredType;
            ImplementationType = implementationType;
        }

        /// <summary> Тип, зарегистрированный в контейнере </summary>
        public Type RegisteredType { get; }

        /// <summary> Фактический тип стрелочника </summary>
        public Type ImplementationType { get; }
    }
}