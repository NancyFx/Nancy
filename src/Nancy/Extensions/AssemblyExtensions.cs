using System;
using System.Reflection;
using System.IO;

namespace Nancy.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] SafeGetExportedTypes(this Assembly assembly)
        {
            Type[] assemblies;

            try
            {
                assemblies = assembly.GetExportedTypes();
            }
            catch (FileNotFoundException)
            {
                assemblies = new Type[] { };
            }

            return assemblies;
        }
    }
}
