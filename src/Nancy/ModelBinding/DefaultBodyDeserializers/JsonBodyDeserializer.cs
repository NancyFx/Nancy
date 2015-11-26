namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System.IO;
    using System.Reflection;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Deserializes request bodies in JSON format
    /// </summary>
    public class JsonBodyDeserializer : IBodyDeserializer
    {
        private readonly MethodInfo deserializeMethod = typeof(JavaScriptSerializer).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public);
        private readonly JsonConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBodyDeserializer"/>,
        /// with the provided <paramref name="environment"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public JsonBodyDeserializer(INancyEnvironment environment)
        {
            this.configuration = environment.GetValue<JsonConfiguration>();
        }

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to deserialize</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(MediaRange mediaRange, BindingContext context)
        {
            return Json.IsJsonContentType(mediaRange);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="mediaRange">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
        {
            var serializer = new JavaScriptSerializer(
                null,
                false,
                this.configuration.MaxJsonLength,
                this.configuration.MaxRecursions,
                this.configuration.RetainCasing,
                this.configuration.UseISO8601DateFormat,
                this.configuration.Converters,
                this.configuration.PrimitiveConverters);

            serializer.RegisterConverters(this.configuration.Converters, this.configuration.PrimitiveConverters);

            bodyStream.Position = 0;
            string bodyText;
            using (var bodyReader = new StreamReader(bodyStream))
            {
                bodyText = bodyReader.ReadToEnd();
            }

            var genericDeserializeMethod = this.deserializeMethod.MakeGenericMethod(context.DestinationType);

            var deserializedObject = genericDeserializeMethod.Invoke(serializer, new object[] { bodyText });

            return deserializedObject;
        }
    }
}
