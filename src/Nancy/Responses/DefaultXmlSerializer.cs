namespace Nancy.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using System.Text;
    using Nancy.Configuration;
    using Nancy.Responses.Negotiation;
    using Nancy.Xml;

    /// <summary>
    /// Default <see cref="ISerializer"/> implementation for XML serialization.
    /// </summary>
    public class DefaultXmlSerializer : ISerializer
    {
        private readonly XmlConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultXmlSerializer"/> class,
        /// with the provided <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultXmlSerializer(INancyEnvironment environment)
        {
            this.configuration = environment.GetValue<XmlConfiguration>();
        }

        /// <summary>
        /// Whether the serializer can serialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to serialise</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanSerialize(MediaRange mediaRange)
        {
            return IsXmlType(mediaRange);
        }

        /// <summary>
        /// Gets the list of extensions that the serializer can handle.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of extensions if any are available, otherwise an empty enumerable.</value>
        public IEnumerable<string> Extensions
        {
            get { yield return "xml"; }
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
            try
            {
                var serializer = new XmlSerializer(typeof(TModel));

                if (this.configuration.EncodingEnabled)
                {
                    serializer.Serialize(new StreamWriter(outputStream, this.configuration.DefaultEncoding), model);
                }
                else
                {
                    serializer.Serialize(outputStream, model);
                }
            }
            catch (Exception exception)
            {
                if (!StaticConfiguration.DisableErrorTraces)
                {
                    var bytes = Encoding.UTF8.GetBytes(exception.Message);
                    outputStream.Write(bytes, 0, exception.Message.Length);
                }
            }
        }

        private static bool IsXmlType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/xml", StringComparison.OrdinalIgnoreCase)
                || contentMimeType.Equals("text/xml", StringComparison.OrdinalIgnoreCase)
                || (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase)
                && contentMimeType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase));
        }
    }
}