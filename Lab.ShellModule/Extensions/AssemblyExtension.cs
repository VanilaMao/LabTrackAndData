using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab.ShellModule.Extensions
{
    public static class AssemblyExtension
    {
        // just once, get all the settings child classes
        public static IEnumerable<Type> GetAllSubClasses(this Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsClass&& ! t.IsAbstract &&
                            t.IsSubclassOf(type));
        }       
    }
}
