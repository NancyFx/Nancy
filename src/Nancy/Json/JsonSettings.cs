namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nancy.Json.Converters;

    /// <summary>
    /// JSON serializer settings
    /// </summary>
    public static class JsonSettings
    {
        private static string _defaultCharset;

        static JsonSettings()
        {
            ISO8601DateFormat = true;
            MaxJsonLength = 102400;
            MaxRecursions = 100;
            DefaultEncoding = Encoding.UTF8;
            Converters = new List<JavaScriptConverter>
            {
                new TimeSpanConverter(),
                new TupleConverter()
            };
            PrimitiveConverters = new List<JavaScriptPrimitiveConverter>();
            RetainCasing = false;
        }

        /// <summary>
        /// Max length of JSON output
        /// </summary>
        public static int MaxJsonLength { get; set; }

        /// <summary>
        /// Maximum number of recursions
        /// </summary>
        public static int MaxRecursions { get; set; }

        /// <summary>
        /// Gets the default encoding for JSON responses.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="Encoding.UTF8" />
        /// </remarks>
        public static Encoding DefaultEncoding { get; set; }

        public static IList<JavaScriptConverter> Converters { get; set; }

        public static IList<JavaScriptPrimitiveConverter> PrimitiveConverters { get; set; }

        /// <summary>
        /// Set to true to retain the casing used in the C# code in produced JSON.
        /// Set to false to use camelCasing in the produced JSON.
        /// False by default.
        /// </summary>
        public static bool RetainCasing { get; set; }

        /// <summary>
        /// Serialized date format
        /// </summary>
        public static bool ISO8601DateFormat { get; set; }
    }
}
