namespace Nancy.Extensions
{
    using System;
    using System.Reflection;
    using System.IO;

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
