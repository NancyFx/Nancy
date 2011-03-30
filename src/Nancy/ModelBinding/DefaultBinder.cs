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

        private readonly IFieldNameConverter fieldNameConverter;

        private readonly BindingDefaults defaults;

        public DefaultBinder(IEnumerable<ITypeConverter> typeConverters, IEnumerable<IBodyDeserializer> bodyDeserializers, IFieldNameConverter fieldNameConverter, BindingDefaults defaults)
        {
            if (typeConverters == null)
            {
                throw new ArgumentNullException("typeConverters");
            }

            if (bodyDeserializers == null)
            {
                throw new ArgumentNullException("bodyDeserializers");
            }

            if (fieldNameConverter == null)
            {
                throw new ArgumentNullException("fieldNameConverter");
            }

            if (defaults == null)
            {
                throw new ArgumentNullException("defaults");
            }

            this.typeConverters = typeConverters;
            this.bodyDeserializers = bodyDeserializers;
            this.fieldNameConverter = fieldNameConverter;
            this.defaults = defaults;
        }

        /// <summary>
        /// Bind to the given model type
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="modelType">Model type to bind to</param>
        /// <param name="blackList">Blacklisted property names</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType, params string[] blackList)
        {
            var result = this.DeserializeRequestBody(context, modelType, blackList);

            if (result != null)
            {
                return result;
            }

            var bindingContext = this.CreateBindingContext(context, modelType, blackList);

            foreach (var modelProperty in bindingContext.ValidModelProperties)
            {
                var stringValue = this.GetValue(modelProperty.Name, bindingContext);

                if (!String.IsNullOrEmpty(stringValue))
                {
                    this.BindProperty(modelProperty, stringValue, bindingContext);
                }
            }

            return bindingContext.Model;
        }

        private BindingContext CreateBindingContext(NancyContext context, Type modelType, string[] blackList)
        {
            return new BindingContext()
                {
                    Context = context,
                    DestinationType = modelType,
                    Model = this.CreateModel(modelType),
                    ValidModelProperties = this.GetProperties(modelType, blackList),
                    FormFields = this.GetFormFields(context),
                    TypeConverters = this.typeConverters.Concat(this.defaults.DefaultTypeConverters),
                };
        }

        private IDictionary<string, string> GetFormFields(NancyContext context)
        {
            var formDictionary = (DynamicDictionary)context.Request.Form;

            return formDictionary.GetDynamicMemberNames().ToDictionary(
                memberName => this.fieldNameConverter.Convert(memberName), 
                memberName => (string)formDictionary[memberName]);
        }

        private void BindProperty(PropertyInfo modelProperty, string stringValue, BindingContext context)
        {
            var destinationType = modelProperty.PropertyType;

            var typeConverter =
                this.typeConverters.Where(c => c.CanConvertTo(destinationType)).FirstOrDefault();

            if (typeConverter != null)
            {
                this.SetPropertyValue(modelProperty, context.Model, typeConverter.Convert(stringValue, destinationType, context));
                return;
            }

            if (destinationType == typeof(string))
            {
                this.SetPropertyValue(modelProperty, context.Model, stringValue);
            }
        }

        private void SetPropertyValue(PropertyInfo modelProperty, object model, object value)
        {
            // TODO - catch reflection exceptions?
            modelProperty.SetValue(model, value, null);
        }

        private IEnumerable<PropertyInfo> GetProperties(Type modelType, string[] blackList)
        {
            return modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && !blackList.Contains(p.Name, StringComparer.InvariantCulture));
        }

        private object CreateModel(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }

        private string GetValue(string propertyName, BindingContext context)
        {
            // TODO - check captured variables too if possible?
            return context.FormFields.ContainsKey(propertyName) ? context.FormFields[propertyName] : String.Empty;
        }

        private object DeserializeRequestBody(NancyContext context, Type modelType, string[] blackList)
        {
            if (context == null || context.Request == null)
            {
                return null;
            }

            var contentType = this.GetRequestContentType(context);
            var bodyDeserializer = this.bodyDeserializers.Where(b => b.CanDeserialize(contentType)).FirstOrDefault();

            if (bodyDeserializer != null)
            {
                return bodyDeserializer.Deserialize(contentType, modelType, context.Request.Body, context);
            }
        
            bodyDeserializer = this.defaults.DefaultBodyDeserializers.Where(b => b.CanDeserialize(contentType)).FirstOrDefault();
            if (bodyDeserializer != null)
            {
                return bodyDeserializer.Deserialize(contentType, modelType, context.Request.Body, context);
            }

            return null;
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