using System;
using System.Reflection;

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
            catch (Exception)
            {
                assemblies = new Type[] { };
            }

            return assemblies;
        }
    }
}
