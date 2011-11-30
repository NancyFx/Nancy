namespace Nancy.Extensions
{
    using System;

    public static class TypeExtensions
    {
        public static string GetAssemblyPath(this Type source)
        {
            var assemblyUri =
                new Uri(source.Assembly.EscapedCodeBase);

            return assemblyUri.LocalPath;
        }
    }
}