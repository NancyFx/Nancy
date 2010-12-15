using System.Xml.Serialization;

namespace Nancy.Formatters.Responses
{
    using System;
    using System.Net;
    using System.IO;

    public class XmlResponse<TModel> : Response
    {
        public XmlResponse(TModel model)
        {
            this.Contents = GetXmlContents(model);
            this.ContentType = "text/xml";
            this.StatusCode = HttpStatusCode.OK;
        }

        private Action<Stream> GetXmlContents(TModel model)
        {
            return stream =>
                        {
                            var serializer = new XmlSerializer(typeof(TModel));
                            serializer.Serialize(stream, model);
                        };
        }
    }
}