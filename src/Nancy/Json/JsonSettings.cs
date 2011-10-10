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

        public static IList<JavaScriptConverter> Converters { get; set; }

        static JsonSettings()
        {
            MaxJsonLength = 102400;
            MaxRecursions = 100;
            Converters = new List<JavaScriptConverter>
                             {
                                 new TimeSpanConverter(),
                             };
        }
    }
}