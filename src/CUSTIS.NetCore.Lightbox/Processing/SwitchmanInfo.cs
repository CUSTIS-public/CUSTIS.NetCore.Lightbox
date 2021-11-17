using System;
using System.Reflection;
using CUSTIS.NetCore.Outbox.Contracts;

namespace CUSTIS.NetCore.Outbox.Processing
{
    /// <summary> Информация о стрелочнике </summary>
    internal class SwitchmanInfo
    {
        /// <summary> Тип обрабатываемого сообщения </summary>
        public string MessageType { get; set; }

        /// <summary> Тип стрелочника (в <see cref="IServiceProvider"/> </summary>
        public Type SwitchmanType { get; set; }

        /// <summary> Метод, обрабатывающий сообщение </summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary> Информация о стрелочнике </summary>
        public SwitchmanInfo(Type switchmanType, SwitchmanAttribute switchmanAttribute, MethodInfo methodInfo)
            : this(switchmanType, switchmanAttribute.MessageType, methodInfo)
        {
        }

        /// <summary> Информация о стрелочнике </summary>
        public SwitchmanInfo(Type switchmanType, string messageType, MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            SwitchmanType = switchmanType;
            MessageType = messageType;
        }
    }
}