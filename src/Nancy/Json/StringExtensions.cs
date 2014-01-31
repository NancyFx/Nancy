namespace Nancy.Json
{
    using System;

    internal static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            return value.Convert(x => x.ToLowerInvariant());
        }

        public static string ToPascalCase(this string value)
        {
            return value.Convert(x => x.ToUpperInvariant());
        }

        private static string Convert(this string value, Func<string, string> converter)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return string.Concat(converter(value.Substring(0, 1)), value.Substring(1));
        }
    }
}