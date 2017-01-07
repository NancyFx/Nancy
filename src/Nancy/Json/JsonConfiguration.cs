namespace Nancy.Json
{
    using System.Collections.Generic;
    using System.Text;
    using Nancy.Json.Converters;

    /// <summary>
    /// Configuration for JSON serialization.
    /// </summary>
    public class JsonConfiguration
    {
        /// <summary>
        /// A default instance of the <see cref="JsonConfiguration"/> class.
        /// </summary>
        public static readonly JsonConfiguration Default = new JsonConfiguration
        {
            Converters = new List<JavaScriptConverter> { new TimeSpanConverter(), new TupleConverter() },
            DefaultEncoding = Encoding.UTF8,
            PrimitiveConverters = new List<JavaScriptPrimitiveConverter>(),
            RetainCasing = false,
            SerializeEnumToString = false,
            ExcludeNullValues = false
        };

        private JsonConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfiguration"/> class.
        /// </summary>
        /// <param name="defaultEncoding">The default <see cref="Encoding"/> that should be used by the serializer.</param>
        /// <param name="converters">List of <see cref="JavaScriptConverter"/> instances.</param>
        /// <param name="primitiveConverters">List of <see cref="JavaScriptPrimitiveConverter"/> instances.</param>
        /// <param name="retainCasing"><see langword="true"/> if the name casing should be retained during serialization, otherwise <see langword="false"/>.</param>
        /// <param name="serializeEnumToString"><see langword="true"/> if enums should be represented as string otherwise <see langword="false"/>.</param>
        /// <param name="excludeNullValues"><see langword="true" /> if the serializer should exclude null values for properties on objects otherwise <see langword="false" />.</param>
        public JsonConfiguration(Encoding defaultEncoding, IList<JavaScriptConverter> converters, IList<JavaScriptPrimitiveConverter> primitiveConverters, bool? retainCasing, bool? serializeEnumToString, bool? excludeNullValues)
        {
            this.DefaultEncoding = defaultEncoding ?? Default.DefaultEncoding;
            this.Converters = converters ?? Default.Converters;
            this.PrimitiveConverters = primitiveConverters ?? Default.PrimitiveConverters;
            this.RetainCasing = retainCasing ?? Default.RetainCasing;
            this.SerializeEnumToString = serializeEnumToString ?? Default.SerializeEnumToString;
            this.ExcludeNullValues = excludeNullValues ?? Default.ExcludeNullValues;
        }

        /// <summary>
        /// Gets the default <see cref="Encoding"/> for JSON responses.
        /// </summary>
        /// <remarks>The default is <see langword="Encoding.UTF8" />.</remarks>
        public Encoding DefaultEncoding { get; private set; }

        /// <summary>
        /// Gets or sets the type converters that should be used.
        /// </summary>
        /// <remarks>The default is <see cref="TimeSpanConverter"/> and <see cref="TupleConverter"/>.</remarks>
        public IList<JavaScriptConverter> Converters { get; private set; }

        /// <summary>
        /// Gets or sets the converters used for primitive types.
        /// </summary>
        /// <remarks>The default are no converters.</remarks>
        public IList<JavaScriptPrimitiveConverter> PrimitiveConverters { get; private set; }

        /// <summary>
        /// Gets or sets if C# casing should be retained or if camel-casing should be enforeced.
        /// </summary>
        /// <remarks>The default is <see langword="false"/>.</remarks>
        public bool RetainCasing { get; private set; }

        /// <summary>
        /// Get or sets whether enums should be treated as string
        /// </summary>
        public bool SerializeEnumToString{ get; private set; }

        /// <summary>
        /// Gets or sets if the serializer should return null values for properties on objects
        /// </summary>
        public bool ExcludeNullValues { get; set; }
    }
}
