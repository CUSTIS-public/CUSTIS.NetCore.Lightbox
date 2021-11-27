using System;
using System.Collections.Concurrent;

namespace CUSTIS.NetCore.Lightbox.Utils
{
    internal class TypeLoader
    {
        private static readonly ConcurrentDictionary<string, Type> Types = new();

        public Type RetrieveType(string fullName)
        {
            return Types.GetOrAdd(fullName, RetrieveTypeInternal);
        }

        private static Type RetrieveTypeInternal(string fullName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(fullName, false);

                if (type != null)
                {
                    return type;
                }
            }

            throw new ArgumentException($"Type {fullName} doesn't exist in the current app domain");
        }
    }
}