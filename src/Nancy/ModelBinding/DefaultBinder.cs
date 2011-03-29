namespace Nancy.ModelBinding
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
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
            // TODO - Blacklist
            // TODO - Name conversion conventions? Might not want to exactly match name in form fields
            // TODO - Collections support

            var result = this.DeserializeRequestBody(context, modelType);

            if (result != null)
            {
                return result;
            }

            var model = this.CreateModel(modelType);

            var modelProperties = this.GetProperties(modelType);

            foreach (var modelProperty in modelProperties)
            {
                var stringValue = this.GetValue(modelProperty.Name, context);

                if (!String.IsNullOrEmpty(stringValue))
                {
                    this.BindProperty(model, modelProperty, stringValue, context);
                }
            }

            return model;
        }

        private void BindProperty(object model, PropertyInfo modelProperty, string stringValue, NancyContext context)
        {
            var destinationType = modelProperty.PropertyType;

            var typeConverter =
                this.typeConverters.Where(c => c.CanConvertTo(destinationType)).FirstOrDefault();

            if (typeConverter != null)
            {
                this.SetPropertyValue(modelProperty, model, typeConverter.Convert(stringValue, destinationType, context));
                return;
            }

            if (destinationType == typeof(string))
            {
                this.SetPropertyValue(modelProperty, model, stringValue);
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(destinationType))
            {
                this.BindCollection(model, modelProperty, stringValue, context);
            }

            var converter = TypeDescriptor.GetConverter(modelProperty.PropertyType);

            if (converter == null || !converter.CanConvertFrom(typeof(string)))
            {
                return;
            }

            try
            {
                this.SetPropertyValue(modelProperty, model, converter.ConvertFrom(stringValue));
            }
            catch (FormatException)
            {
            }
        }

        private void BindCollection(object model, PropertyInfo modelProperty, string stringValue, NancyContext context)
        {
            throw new NotImplementedException();
        }

        private void SetPropertyValue(PropertyInfo modelProperty, object model, object value)
        {
            // TODO - catch reflection exceptions?
            modelProperty.SetValue(model, value, null);
        }

        private IEnumerable<PropertyInfo> GetProperties(Type modelType)
        {
            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);
        }

        private object CreateModel(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }

        private string GetValue(string propertyName, NancyContext context)
        {
            // TODO - check captured variables too if possible
            var dictionaryItem = (DynamicDictionaryValue)context.Request.Form[propertyName];

            return dictionaryItem.HasValue ? dictionaryItem : String.Empty;
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