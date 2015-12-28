namespace Nancy.Demo.Hosting.Aspnet
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// If you request /negotiated with an accept header with the value image/png you will get an image back
    /// </summary>
    public class PngSerializer : ISerializer
    {
        private readonly IRootPathProvider rootPathProvider;

        public PngSerializer(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(MediaRange mediaRange)
        {
            return mediaRange.Matches("image/png");
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get { yield return "png"; }
        }

        /// <summary>
        /// Serialize the given model with the given contentType
        /// </summary>
        /// <param name="mediaRange">Content type to serialize into</param>
        /// <param name="model">Model to serialize</param>
        /// <param name="outputStream">Output stream to serialize to</param>
        /// <returns>Serialised object</returns>
        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            var path =
                Path.Combine(this.rootPathProvider.GetRootPath(), "content/face.png");

            var face =
                Image.FromFile(path);

            face.Save(outputStream, ImageFormat.Png);
        }
    }
}