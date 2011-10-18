namespace Nancy.Responses
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public class XmlResponse<TModel> : Response
    {
        public XmlResponse(TModel model, string contentType)
        {
            if (DefaultSerializersStartup.XmlSerializer == null)
            {
                throw new InvalidOperationException("XML Serializer not set");
            }

            this.Contents = GetXmlContents(model);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetXmlContents(TModel model)
        {
            return stream => DefaultSerializersStartup.XmlSerializer.Serialize("application/xml", model, stream);
        }
    }
}