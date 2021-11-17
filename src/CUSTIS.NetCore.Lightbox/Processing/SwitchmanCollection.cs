using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CUSTIS.NetCore.Outbox.Contracts;

namespace CUSTIS.NetCore.Outbox.Processing
{
    /// <summary> Коллекция стрелочников </summary>
    internal class SwitchmanCollection : ISwitchmanCollection
    {
        private readonly IDictionary<string, SwitchmanInfo> _switchmansByType;

        /// <summary> Коллекция стрелочников </summary>
        public SwitchmanCollection(IEnumerable<SwitchmanType> switchmanTypes)
        {
            var switchmanInfos = new List<SwitchmanInfo>();
            var validator = new SwitchmanValidator();

            foreach (var switchmanType in switchmanTypes)
            {
                var implType = switchmanType.ImplementationType;

                var methodInfos = implType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                foreach (var methodInfo in methodInfos)
                {
                    var attr = methodInfo.GetCustomAttribute<SwitchmanAttribute>();

                    if (attr == null)
                    {
                        continue;
                    }

                    if (!validator.Validate(methodInfo, attr))
                    {
                        continue;
                    }

                    switchmanInfos.Add(new SwitchmanInfo(switchmanType.RegisteredType, attr, methodInfo));
                }
            }

            validator.Validate(switchmanInfos);

            validator.ThrowIfNecessary();

            _switchmansByType = switchmanInfos.ToDictionary(s => s.MessageType);
        }

        /// <summary> Получить стрелочника по типу сообщения </summary>
        public SwitchmanInfo Get(string messageType)
        {
            return _switchmansByType[messageType];
        }
    }
}