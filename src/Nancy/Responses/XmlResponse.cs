namespace Nancy.Responses
{
    using System;
    using System.IO;
    using Nancy.Configuration;
    using Nancy.Xml;

    public class XmlResponse<TModel> : Response
    {
        private readonly XmlConfig config;

        public XmlResponse(TModel model, ISerializer serializer, INancyEnvironment environment)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("XML Serializer not set");
            }

            this.config = environment.GetValue<XmlConfig>();

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
                return this.config.EncodingEnabled
                    ? string.Concat("; charset=", this.config.DefaultEncoding.WebName)
                    : string.Empty;
            }
        }

        private Action<Stream> GetXmlContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize(this.DefaultContentType, model, stream);
        }
    }
}
