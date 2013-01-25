namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public class XmlBodyDeserializer : IBodyDeserializer
    {
        public bool CanDeserialize(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/xml", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.Equals("text/xml", StringComparison.InvariantCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.InvariantCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+xml", StringComparison.InvariantCultureIgnoreCase));
        }

        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            var ser = new XmlSerializer(context.DestinationType);
            return ser.Deserialize(bodyStream);
        }
    }
}