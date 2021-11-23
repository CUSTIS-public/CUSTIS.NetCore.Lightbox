using System;

namespace CUSTIS.NetCore.Lightbox.Utils
{
    /// <summary> Сериализация и десерилазиация объектов </summary>
    /// <remarks> Интерфейс понадобился, поскольку разные потребители сидят на разных версиях Newtonsoft.JSON </remarks>
    public interface IJsonConvert
    {
        /// <summary> Десериализовать </summary>
        object? Deserialize(string value, Type type);

        /// <summary> Сериализовать </summary>
        string Serialize(object? value);
    }
}