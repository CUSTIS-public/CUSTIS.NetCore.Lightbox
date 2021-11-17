using System;

namespace CUSTIS.NetCore.Outbox.Contracts
{
    /// <summary> Размечает метод, переадресующий сообщения из Outbox в нужную систему </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwitchmanAttribute : Attribute
    {
        /// <summary> Размечает метод, переадресующий сообщения из Outbox в нужную систему </summary>
        public SwitchmanAttribute(string messageType)
        {
            MessageType = messageType;
        }

        /// <summary> Тип обрабатываемого сообщения </summary>
        public string MessageType { get; }
    }
}