namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default binder - used as a fallback when a specific modelbinder
    /// is not available.
    /// </summary>
    public class DefaultBinder : IBinder
    {
        private readonly IEnumerable<ITypeConverter> typeConverters;

        private readonly IEnumerable<IBodyDeserializer> bodyDeserializers;

        public DefaultBinder(IEnumerable<ITypeConverter> typeConverters, IEnumerable<IBodyDeserializer> bodyDeserializers)
        {
            if (typeConverters == null)
            {
                throw new ArgumentNullException("typeConverters");
            }

            if (bodyDeserializers == null)
            {
                throw new ArgumentNullException("bodyDeserializers");
            }

            this.typeConverters = typeConverters;
            this.bodyDeserializers = bodyDeserializers;
        }

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType)
        {
            var result = this.DeserializeRequestBody(context, modelType);

            if (result != null)
            {
                return result;
            }

            // Currently this is *extremely* dumb and PoC only :-)
            // Models must have a default constructor and public settable properties
            // for each member. Also only supports very basic types and only works
            // if the input type is directly assignable to the type in the object.
            var model = Activator.CreateInstance(modelType);

            var modelProperties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);
            DynamicDictionary formDictionary = context.Request.Form;

            foreach (var modelProperty in modelProperties)
            {
                var dictionaryItem = (DynamicDictionaryValue)formDictionary[modelProperty.Name];

                // TODO - Use type converters if they are available
                if (dictionaryItem.HasValue)
                {
                    if (modelProperty.PropertyType.IsAssignableFrom(dictionaryItem.Value.GetType()))
                    {
                        modelProperty.SetValue(model, dictionaryItem.Value, null);
                    }
                    else
                    {
                        //modelProperty.SetValue(model, dictionaryItem, null);
                    }
                }
            }

            return model;
        }

        private object DeserializeRequestBody(NancyContext context, Type modelType)
        {
            if (context == null || context.Request == null)
            {
                return null;
            }

            var contentType = this.GetRequestContentType(context);

            var bodyDeserializer = this.bodyDeserializers.Where(b => b.CanDeserialize(contentType)).FirstOrDefault();

            return bodyDeserializer != null
                       ? bodyDeserializer.Deserialize(contentType, context.Request.Body, context)
                       : null;
        }

        private string GetRequestContentType(NancyContext context)
        {
            if (context == null || context.Request == null)
            {
                return String.Empty;
            }

            IEnumerable<string> contentTypeHeaders;
            context.Request.Headers.TryGetValue("Content-Type", out contentTypeHeaders);

            if (contentTypeHeaders == null || !contentTypeHeaders.Any())
            {
                return string.Empty;
            }

            return contentTypeHeaders.First();
        }
    }
}