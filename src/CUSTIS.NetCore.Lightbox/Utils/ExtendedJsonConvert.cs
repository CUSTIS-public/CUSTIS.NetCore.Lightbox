using System;
using Newtonsoft.Json;

namespace CUSTIS.NetCore.Utils.Serialization
{
    /// <summary> Расширенный JsonConvert </summary>
    internal static class ExtendedJsonConvert
    {
        /// <summary> Десериализовать или кинуть Exception</summary>
        public static T Deserialize<T>(string value)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(value);

                if (obj is null)
                {
                    throw new InvalidOperationException($"Не удалось десериализовать строку {value} в объект типа {typeof(T)}");
                }

                return obj;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Не удалось десериализовать строку {value} в объект типа {typeof(T)}", e);
            }
        }

        /// <summary> Сериализовать или кинуть Exception</summary>
        public static string Serialize<T>(T value)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Не удалось сериализовать {value} в json", e);
            }
        }
    }
}