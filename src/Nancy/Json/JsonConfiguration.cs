namespace Nancy.Json
{
    using System.Collections.Generic;
    using System.Text;
    using Nancy.Json.Converters;

    /// <summary>
    /// JSON serializer settings
    /// </summary>
    public class JsonConfiguration
    {
        private string defaultCharset;

        /// <summary>
        ///Initializes a new instance of the <see cref="JsonConfiguration"/> class.
        /// </summary>
        public JsonConfiguration()
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
        public int MaxJsonLength { get; private set; }

        /// <summary>
        /// Maximum number of recursions
        /// </summary>
        public int MaxRecursions { get; private set; }

        /// <summary>
        /// Gets the default encoding for JSON responses.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="Encoding.UTF8" />
        /// </remarks>
        public Encoding DefaultEncoding { get; private set; }

        public IList<JavaScriptConverter> Converters { get; private set; }

        public IList<JavaScriptPrimitiveConverter> PrimitiveConverters { get; private set; }

        /// <summary>
        /// Set to true to retain the casing used in the C# code in produced JSON.
        /// Set to false to use camelCasing in the produced JSON.
        /// False by default.
        /// </summary>
        public bool RetainCasing { get; set; }

        /// <summary>
        /// Serialized date format
        /// </summary>
        public bool ISO8601DateFormat { get; private set; }
    }
}
