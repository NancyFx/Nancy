namespace Nancy.Responses
{
    using System;
    using System.IO;

    public class XmlResponse<TModel> : Response
    {
        public XmlResponse(TModel model, string contentType, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("XML Serializer not set");
            }

            this.Contents = GetXmlContents(model, serializer);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetXmlContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize("application/xml", model, stream);
        }
    }
}