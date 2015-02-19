namespace Nancy.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Nancy.IO;
    using Nancy.Json;

    public class DefaultJsonSerializer : ISerializer
    {
        private bool? retainCasing;
        private bool? iso8601DateFormat;

        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="contentType">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(string contentType)
        {
            return IsJsonType(contentType);
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get
            {
                yield return "json";
            }
        }

        /// <summary>
        /// Set to true to retain the casing used in the C# code in produced JSON.
        /// Set to false to use camelCasig in the produced JSON.
        /// False by default.
        /// </summary>
        public bool RetainCasing
        {
            get { return retainCasing.HasValue ? retainCasing.Value : JsonSettings.RetainCasing; }
            set { retainCasing = value; }
        }

        /// <summary>
        /// Set to true to use the ISO8601 format for datetimes in produced JSON.
        /// Set to false to use the WCF \/Date()\/ format in the produced JSON.
        /// True by default.
        /// </summary>
        public bool ISO8601DateFormat
        {
            get { return iso8601DateFormat.HasValue ? iso8601DateFormat.Value : JsonSettings.ISO8601DateFormat; }
            set { iso8601DateFormat = value; }
        }

        /// <summary>
        /// Serialize the given model with the given contentType
        /// </summary>
        /// <param name="contentType">Content type to serialize into</param>
        /// <param name="model">Model to serialize</param>
        /// <param name="outputStream">Stream to serialize to</param>
        /// <returns>Serialised object</returns>
        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            using (var writer = new StreamWriter(new UnclosableStreamWrapper(outputStream)))
            {
                var serializer = new JavaScriptSerializer(null, false, JsonSettings.MaxJsonLength, JsonSettings.MaxRecursions, RetainCasing, ISO8601DateFormat);

                serializer.RegisterConverters(JsonSettings.Converters, JsonSettings.PrimitiveConverters);

                try
                {
                    serializer.Serialize(model, writer);
                }
                catch (Exception exception)
                {
                    if (!StaticConfiguration.DisableErrorTraces)
                    {
                        writer.Write(exception.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to detect if the content type is JSON.
        /// Supports:
        ///   application/json
        ///   text/json
        ///   application/vnd[something]+json
        /// Matches are case insentitive to try and be as "accepting" as possible.
        /// </summary>
        /// <param name="contentType">Request content type</param>
        /// <returns>True if content type is JSON, false otherwise</returns>
        private static bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.OrdinalIgnoreCase) ||
            contentMimeType.StartsWith("application/json-", StringComparison.OrdinalIgnoreCase) ||
            contentMimeType.Equals("text/json", StringComparison.OrdinalIgnoreCase) ||
            (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase) &&
            contentMimeType.EndsWith("+json", StringComparison.OrdinalIgnoreCase));
        }
    }
}
