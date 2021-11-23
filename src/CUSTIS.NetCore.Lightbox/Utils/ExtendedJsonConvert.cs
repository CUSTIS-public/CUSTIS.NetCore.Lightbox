using System;

namespace CUSTIS.NetCore.Lightbox.Utils
{
    /// <summary> Расширенный JsonConvert </summary>
    internal class ExtendedJsonConvert
    {
        private readonly IJsonConvert _jsonConvert;

        /// <summary> Расширенный JsonConvert </summary>
        public ExtendedJsonConvert(IJsonConvert jsonConvert)
        {
            _jsonConvert = jsonConvert;
        }

        /// <summary> Десериализовать или кинуть Exception</summary>
        public object Deserialize(string value, Type type)
        {
            try
            {
                var obj = _jsonConvert.Deserialize(value, type);

                if (obj is null)
                {
                    throw new InvalidOperationException($"Не удалось десериализовать строку {value} в объект типа {type}");
                }

                return obj;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Не удалось десериализовать строку {value} в объект типа {type}", e);
            }
        }

        /// <summary> Десериализовать или кинуть Exception</summary>
        public T Deserialize<T>(string value)
        {
            return (T)Deserialize(value, typeof(T));
        }

        /// <summary> Сериализовать или кинуть Exception</summary>
        public string Serialize<T>(T value)
        {
            try
            {
                return _jsonConvert.Serialize(value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Не удалось сериализовать {value} в json", e);
            }
        }
    }
}