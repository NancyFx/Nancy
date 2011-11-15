namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Json;

    /// <summary>
    /// Deserializes request bodies in JSON format
    /// </summary>
    public class JsonBodyDeserializer : IBodyDeserializer
    {
        private readonly MethodInfo deserializeMethod = typeof(JavaScriptSerializer).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType)
        {
            return Json.IsJsonContentType(contentType);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current context</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            var serializer = new JavaScriptSerializer(null, false, JsonSettings.MaxJsonLength, JsonSettings.MaxRecursions);
            serializer.RegisterConverters(JsonSettings.Converters);

            bodyStream.Position = 0;
            string bodyText;
            using (var bodyReader = new StreamReader(bodyStream))
            {
                bodyText = bodyReader.ReadToEnd();
            }

            var genericDeserializeMethod = this.deserializeMethod.MakeGenericMethod(new[] { context.DestinationType });

            var deserializedObject = genericDeserializeMethod.Invoke(serializer, new[] { bodyText });

            if (context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Except(context.ValidModelProperties).Any())
            {
                return this.CreateObjectWithBlacklistExcluded(context, deserializedObject);
            }

            return deserializedObject;
        }

        private object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType);

            foreach (var property in context.ValidModelProperties)
            {
                this.CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private void CopyPropertyValue(PropertyInfo property, object sourceObject, object destinationObject)
        {
            property.SetValue(destinationObject, property.GetValue(sourceObject, null), null);
        }
    }
}