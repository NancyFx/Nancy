namespace Nancy.ModelBinding
{
    using System.IO;

    /// <summary>
    /// Provides a way to deserialize the contents of a request
    /// into a bound model.
    /// </summary>
    public interface IBodyDeserializer
    {
        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <returns>True if supported, false otherwise</returns>
        bool CanDeserialize(string contentType);

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        object Deserialize(string contentType, Stream bodyStream, BindingContext context);
    }
}