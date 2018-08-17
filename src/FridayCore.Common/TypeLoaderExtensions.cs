using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FridayCore
{
    public static class TypeLoaderExtensions
    {
        public static IList<Type> GetLoadableTypes(this IEnumerable<Assembly> assemblies)
        {
            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                if (assembly == null) throw new ArgumentNullException("assembly");
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException e)
                {
                    types.AddRange(e.Types.Where(t => t != null));
                }
            }

            return types;
        }
    }
}
