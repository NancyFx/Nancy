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
            RetainCasing = false
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
        public JsonConfiguration(Encoding defaultEncoding, IList<JavaScriptConverter> converters, IList<JavaScriptPrimitiveConverter> primitiveConverters, bool? retainCasing)
        {
            this.DefaultEncoding = defaultEncoding ?? Default.DefaultEncoding;
            this.Converters = converters ?? Default.Converters;
            this.PrimitiveConverters = primitiveConverters ?? Default.PrimitiveConverters;
            this.RetainCasing = retainCasing ?? Default.RetainCasing;
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
    }
}
