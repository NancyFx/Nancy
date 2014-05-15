namespace Nancy.Responses
{
    using System;
    using System.IO;

    using Nancy.Xml;

	public class XmlResponse<TModel> : Response
    {
		private static string contentType
		{
			get
			{
				return "application/xml" + (!XmlSettings.EncodingEnabled ? "" : "; charset=" + XmlSettings.DefaultEncoding.WebName);
			}
		}

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
			return stream => serializer.Serialize(contentType, model, stream);
        }
    }
}