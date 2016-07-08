namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Nancy.Configuration;
    using Nancy.Xml;

    /// <summary>
    /// Represents an HTTP response with XML content.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <seealso cref="Nancy.Response" />
    public class XmlResponse<TModel> : Response
    {
        private readonly XmlConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResponse{TModel}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="environment">The environment.</param>
        /// <exception cref="System.InvalidOperationException">XML Serializer not set</exception>
        public XmlResponse(TModel model, ISerializer serializer, INancyEnvironment environment)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("XML Serializer not set");
            }

            this.configuration = environment.GetValue<XmlConfiguration>();

            this.Contents = GetXmlContents(model, serializer);
            this.ContentType = DefaultContentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private string DefaultContentType
        {
            get { return string.Concat("application/xml", this.Encoding); }
        }

        private string Encoding
        {
            get
            {
                return this.configuration.EncodingEnabled
                    ? string.Concat("; charset=", this.configuration.DefaultEncoding.WebName)
                    : string.Empty;
            }
        }

        private Action<Stream> GetXmlContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize(this.DefaultContentType, model, stream);
        }
    }
}
