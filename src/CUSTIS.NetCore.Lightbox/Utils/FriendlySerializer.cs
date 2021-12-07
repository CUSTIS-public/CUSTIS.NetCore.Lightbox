using System;

namespace CUSTIS.NetCore.Lightbox.Utils
{
    /// <summary> Сериализатор, который кидает понятные разработчику исключения </summary>
    internal class FriendlySerializer
    {
        private readonly ISerializer _serializer;

        /// <summary> Сериализатор, который кидает понятные разработчику исключения </summary>
        public FriendlySerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <summary> Десериализовать или кинуть Exception</summary>
        public object Deserialize(string value, Type type)
        {
            try
            {
                var obj = _serializer.Deserialize(value, type);

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
                return _serializer.Serialize(value);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Не удалось сериализовать {value} в json", e);
            }
        }
    }
}