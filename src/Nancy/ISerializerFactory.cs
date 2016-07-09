namespace Nancy
{
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Defines the functionality of an <see cref="ISerializer"/> factory.
    /// </summary>
    public interface ISerializerFactory
    {
        /// <summary>
        /// Gets the <see cref="ISerializer"/> implementation that can serialize the provided <paramref name="mediaRange"/>.
        /// </summary>
        /// <param name="mediaRange">The <see cref="MediaRange"/> to get a serializer for.</param>
        /// <returns>An <see cref="ISerializer"/> instance, or <see langword="null" /> if not match was found.</returns>
        ISerializer GetSerializer(MediaRange mediaRange);
    }
}