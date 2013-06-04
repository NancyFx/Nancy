namespace Nancy.Json
{
    using System.Collections.Generic;
    using Converters;
    using Responses;

    /// <summary>
    /// Json serializer settings
    /// </summary>
    public static class JsonSettings
    {
        private static string _defaultsCharset;

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
        public static string DefaultCharset 
        { 
            get
            {
                return _defaultsCharset;
            } 
            set
            {
                _defaultsCharset = value;
                JsonResponse.CharSet = value;
            } 
        }

        public static IList<JavaScriptConverter> Converters { get; set; }

        static JsonSettings()
        {
            MaxJsonLength = 102400;
            MaxRecursions = 100;
            DefaultCharset = "utf8";
            Converters = new List<JavaScriptConverter>
                             {
                                 new TimeSpanConverter(),
                             };
        }
    }
}