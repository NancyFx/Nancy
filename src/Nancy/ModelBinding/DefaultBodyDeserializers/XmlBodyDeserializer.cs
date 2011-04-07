using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
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
            var attributeOverrides = new XmlAttributeOverrides();
            var propertiesToIgnore = context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                    .Except(context.ValidModelProperties);
            foreach (var validModelProperty in propertiesToIgnore)
            {
                attributeOverrides.Add(context.DestinationType, validModelProperty.Name, new XmlAttributes{XmlIgnore = true});
            }

            var ser = new XmlSerializer(context.DestinationType, attributeOverrides);
            return ser.Deserialize(bodyStream);
        }
    }
}