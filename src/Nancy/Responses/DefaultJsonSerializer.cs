namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Json;

    using Nancy.IO;

    public class DefaultJsonSerializer : ISerializer
    {
        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="contentType">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(string contentType)
        {
            return this.IsJsonType(contentType);
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
                var serializer = new JavaScriptSerializer(null, false, JsonSettings.MaxJsonLength, JsonSettings.MaxRecursions);
            
                serializer.RegisterConverters(JsonSettings.Converters);

                serializer.Serialize(model, writer);
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
        private bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.InvariantCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+json", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}