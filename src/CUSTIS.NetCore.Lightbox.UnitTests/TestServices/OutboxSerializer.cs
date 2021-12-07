using System;
using CUSTIS.NetCore.Lightbox.Utils;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Lightbox.UnitTests.TestServices
{
    public class OutboxSerializer : ISerializer
    {
        /// <summary> Десериализовать </summary>
        public object? Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        /// <summary> Сериализовать </summary>
        public string Serialize(object? value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}