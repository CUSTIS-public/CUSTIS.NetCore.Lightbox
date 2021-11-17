using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace CUSTIS.NetCore.Utils.Reflection
{
    /// <summary>Вспомогательный класс для работы с Reflection</summary>
    internal static class ReflectionHelper
    {
        /// <summary> Является ли метод асинхронным? </summary>
        /// <remarks> https://stackoverflow.com/a/20350646/4571173 </remarks>
        public static bool IsAsyncMethod(this Type classType, string methodName)
        {
            // Obtain the method with the specified name.
            var method = classType.GetMethod(methodName) ??
                         throw new NoNullAllowedException(
                             $"Не удалось найти метод [{methodName}] в типе [{classType.Name}]");

            return IsAsyncMethod(method);
        }

        /// <summary> Является ли метод асинхронным? </summary>
        /// <remarks> https://stackoverflow.com/a/20350646/4571173 </remarks>
        public static bool IsAsyncMethod(this MethodInfo method)
        {
            var attType = typeof(AsyncStateMachineAttribute);

            // Obtain the custom attribute for the method.
            // The value returned contains the StateMachineType property.
            // Null is returned if the attribute isn't present for the method.
            return (AsyncStateMachineAttribute?)method.GetCustomAttribute(attType) != null;
        }

        /// <summary> Получить название члена класса в формате ИмяКласса.ИмяЧлена </summary>
        public static string GetMemberName(this MemberInfo memberInfo)
        {
            return $"{memberInfo.DeclaringType?.Name}.{memberInfo.Name}";
        }
    }
}