using System;
using System.Collections.Generic;
using System.Linq;

namespace CUSTIS.NetCore.Lightbox.Utils
{
    /// <summary>
    ///     Методы расширения для <see cref="IEnumerable{T}" />
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        ///     Составляет строку из элементов последовательности с указанным разделителем
        /// </summary>
        /// <remarks>
        ///     Более удобная форма записи
        /// </remarks>
        public static string ToJoinedString<T>(this IEnumerable<T> enumerable, string separator = ", ")
        {
            return string.Join(separator, enumerable);
        }

        /// <summary>
        ///     Составляет строку из элементов последовательности с указанным разделителем
        /// </summary>
        /// <remarks>
        ///     Более удобная форма записи
        /// </remarks>
        public static string ToJoinedString<T>(this IEnumerable<T> enumerable, Func<T, string?> selector, string separator = ", ")
        {
            return enumerable.Select(selector).ToJoinedString(separator);
        }
    }
}