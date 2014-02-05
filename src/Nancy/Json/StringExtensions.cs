namespace Nancy.Json
{
    using System;

    internal static class StringExtensions
    {
        /// <summary>
        /// Converts the value from PascalCase to camelCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ToCamelCase(this string value)
        {
            return value.ConvertFirstCharacter(x => x.ToLowerInvariant());
        }

        /// <summary>
        /// Converts the value from camelCase to PascalCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ToPascalCase(this string value)
        {
            return value.ConvertFirstCharacter(x => x.ToUpperInvariant());
        }

        private static string ConvertFirstCharacter(this string value, Func<string, string> converter)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return string.Concat(converter(value.Substring(0, 1)), value.Substring(1));
        }
    }
}