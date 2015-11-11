namespace Nancy
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Defines the functionality for providing serialization support.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        bool CanSerialize(MediaRange mediaRange);

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        IEnumerable<string> Extensions { get; }

        /// <summary>
        /// Serialize the given model with the given contentType
        /// </summary>
        /// <param name="mediaRange">Content type to serialize into</param>
        /// <param name="model">Model to serialize</param>
        /// <param name="outputStream">Output stream to serialize to</param>
        /// <returns>Serialised object</returns>
        void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream);
    }
}