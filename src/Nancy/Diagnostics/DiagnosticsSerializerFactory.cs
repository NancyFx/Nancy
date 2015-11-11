namespace Nancy.Diagnostics
{
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;

    internal class DiagnosticsSerializerFactory : ISerializerFactory
    {
        private readonly ISerializer serializer;

        public DiagnosticsSerializerFactory()
        {
            this.serializer = new DefaultJsonSerializer { RetainCasing = false };
        }

        /// <summary>
        /// Gets the <see cref="ISerializer"/> implementation that can serialize the provided <paramref name="mediaRange"/>.
        /// </summary>
        /// <param name="mediaRange">The <see cref="MediaRange"/> to get a serializer for.</param>
        /// <returns>An <see cref="ISerializer"/> instance, or <see langword="null" /> if not match was found.</returns>
        public ISerializer GetSerializer(MediaRange mediaRange)
        {
            return this.serializer;
        }
    }
}