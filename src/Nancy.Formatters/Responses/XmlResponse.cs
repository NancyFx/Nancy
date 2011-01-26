namespace Nancy.Formatters.Responses
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;

    public class XmlResponse<TModel> : Response
    {
        public XmlResponse(TModel model, string contentType)
        {
            this.Contents = GetXmlContents(model);
            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetXmlContents(TModel model)
        {
            return stream =>
            {
                var serializer = new XmlSerializer(typeof(TModel));
                serializer.Serialize(stream, model);
            };
        }
    }
}