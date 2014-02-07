namespace Nancy.Json
{
    using System.Collections.Generic;
    using Converters;

    /// <summary>
    /// Json serializer settings
    /// </summary>
    public static class JsonSettings
    {
        /// <summary>
        /// Max length of json output
        /// </summary>
        public static int MaxJsonLength { get; set; }

        /// <summary>
        /// Maximum number of recursions
        /// </summary>
        public static int MaxRecursions { get; set; }

        /// <summary>
        /// Default charset for json responses.
        /// </summary>
        public static string DefaultCharset { get; set; }

        public static IList<JavaScriptConverter> Converters { get; set; }
        
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

        static JsonSettings()
        {
            ISO8601DateFormat = true;
            MaxJsonLength = 102400;
            MaxRecursions = 100;
            DefaultCharset = "utf-8";
            Converters = new List<JavaScriptConverter>
                             {
                                 new TimeSpanConverter(),
                             };
            RetainCasing = false;
        }
    }
}
