namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;

    public class JsonBodyDeserializer : IBodyDeserializer
    {
        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType)
        {
            return false;
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="outputType">Output type to deserialize to</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Type outputType, Stream bodyStream, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}