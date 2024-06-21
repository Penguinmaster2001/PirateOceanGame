
using System;
using System.Collections.Generic;

namespace Registries
{
    // Holds unique the ID's 
    public static class GlobalRegistry
    {
        private static readonly Dictionary<Type, object> registries = new();

        internal static Registry<T> GetRegistry<T>() where T : IRegisterable
        {
            Type type = typeof(T);

            if (!registries.TryGetValue(type, out object registry))
            {
                var newRegistry = new Registry<T>();
                registries[type] = newRegistry;
                return newRegistry;
            }

            return (Registry<T>) registry;
        }



        public static int Register<T>(T instance) where T : IRegisterable
        {
            return GetRegistry<T>().Register(instance);
        }
    }
}
