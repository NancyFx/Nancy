namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Nancy.Extensions;

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
        /// <param name="instance">Optional existing instance</param>
        /// <param name="configuration">The <see cref="BindingConfig"/> that should be applied during binding.</param>
        /// <param name="blackList">Blacklisted property names</param>
        /// <returns>Bound model</returns>
        public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
        {
            var bindingContext =
                this.CreateBindingContext(context, modelType, instance, configuration, blackList);

            var bodyDeserializedModel = this.DeserializeRequestBody(bindingContext);

            if (bodyDeserializedModel != null)
            {
                this.UpdateModelWithDeserializedModel(bodyDeserializedModel, bindingContext);
            }

            var bindingExceptions = new List<PropertyBindingException>();
            foreach (var modelProperty in bindingContext.ValidModelProperties)
            {
                var existingValue =
                    modelProperty.GetValue(bindingContext.Model, null);

                var stringValue = GetValue(modelProperty.Name, bindingContext);

                if (!String.IsNullOrEmpty(stringValue) &&  (IsDefaultValue(existingValue, modelProperty.PropertyType) || bindingContext.Overwrite ))
                {
                    try
                    {
                        this.BindProperty(modelProperty, stringValue, bindingContext);
                    }
                    catch(PropertyBindingException ex)
                    {
                        bindingExceptions.Add(ex);
                    }
                }
            }

            if (bindingExceptions.Any())
            {
                throw new ModelBindingException(modelType, bindingExceptions);
            }

            return bindingContext.Model;
        }

        private void UpdateModelWithDeserializedModel(object bodyDeserializedModel, BindingContext bindingContext)
        {
            if (bodyDeserializedModel.GetType().IsCollection() || bodyDeserializedModel.GetType().IsEnumerable())
            {
                bindingContext.Model = bodyDeserializedModel;
            }

            foreach (var modelProperty in bindingContext.ValidModelProperties)
            {
                var existingValue =
                    modelProperty.GetValue(bindingContext.Model, null);

                if (IsDefaultValue(existingValue, modelProperty.PropertyType) || bindingContext.Overwrite)
                {
                    CopyValue(modelProperty, bodyDeserializedModel, bindingContext.Model);
                }
            }
        }

        private static void CopyValue(PropertyInfo modelProperty, object bodyDeserializedModel, object model)
        {
            var newValue = modelProperty.GetValue(bodyDeserializedModel, null);

            modelProperty.SetValue(model, newValue, null);
        }

        private bool IsDefaultValue(object existingValue, Type propertyType)
        {
            return propertyType.IsValueType
                ? Equals(existingValue, Activator.CreateInstance(propertyType))
                : existingValue == null;
        }

        private BindingContext CreateBindingContext(NancyContext context, Type modelType, object instance, BindingConfig configuration, IEnumerable<string> blackList)
        {
            return new BindingContext
            {
                Overwrite = configuration.Overwrite,
                Context = context,
                DestinationType = modelType,
                Model = CreateModel(modelType, instance),
                ValidModelProperties = GetProperties(modelType, blackList),
                RequestData = this.GetDataFields(context),
                TypeConverters = this.typeConverters.Concat(this.defaults.DefaultTypeConverters),
            };
        }

        private IDictionary<string, string> GetDataFields(NancyContext context)
        {
            var dictionaries = new IDictionary<string, string>[]
                {
                    ConvertDynamicDictionary(context.Request.Form), 
                    ConvertDynamicDictionary(context.Request.Query), 
                    ConvertDynamicDictionary(context.Parameters)
                };

            return dictionaries.Merge();
        }

        private IDictionary<string, string> ConvertDynamicDictionary(DynamicDictionary dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            return dictionary.GetDynamicMemberNames().ToDictionary(
                    memberName => this.fieldNameConverter.Convert(memberName),
                    memberName => (string)dictionary[memberName]);
        }

        private static void BindProperty(PropertyInfo modelProperty, string stringValue, BindingContext context)
        {
            var destinationType = modelProperty.PropertyType;

            var typeConverter =
                context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(destinationType, context));

            if (typeConverter != null)
            {
                try
                {
                    SetPropertyValue(modelProperty, context.Model, typeConverter.Convert(stringValue, destinationType, context));
                }
                catch(Exception e)
                {
                    throw new PropertyBindingException(modelProperty.Name, stringValue, e);
                }
            }
            else if (destinationType == typeof(string))
            {
                SetPropertyValue(modelProperty, context.Model, stringValue);
            }
        }

        private static void SetPropertyValue(PropertyInfo modelProperty, object model, object value)
        {
            // TODO - catch reflection exceptions?
            modelProperty.SetValue(model, value, null);
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type modelType, IEnumerable<string> blackList)
        {
            return modelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && !blackList.Contains(p.Name, StringComparer.InvariantCulture))
                .Where(property => !property.GetIndexParameters().Any());
        }

        private static object CreateModel(Type modelType, object instance)
        {
            if (instance == null)
            {
                return Activator.CreateInstance(modelType);
            }

            return !modelType.IsInstanceOfType(instance) ? 
                Activator.CreateInstance(modelType) :
                instance;
        }

        private static string GetValue(string propertyName, BindingContext context)
        {
            return context.RequestData.ContainsKey(propertyName) ? context.RequestData[propertyName] : String.Empty;
        }

        private object DeserializeRequestBody(BindingContext context)
        {
            if (context.Context == null || context.Context.Request == null)
            {
                return null;
            }

            var contentType = GetRequestContentType(context.Context);
            var bodyDeserializer = this.bodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType));

            if (bodyDeserializer != null)
            {
                return bodyDeserializer.Deserialize(contentType, context.Context.Request.Body, context);
            }

            bodyDeserializer = this.defaults.DefaultBodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType));
            
            return bodyDeserializer != null ? 
                bodyDeserializer.Deserialize(contentType, context.Context.Request.Body, context) : 
                null;
        }

        private static string GetRequestContentType(NancyContext context)
        {
            if (context == null || context.Request == null)
            {
                return String.Empty;
            }

            var contentType =
                context.Request.Headers.ContentType;

            return (string.IsNullOrEmpty(contentType))
                ? string.Empty
                : contentType;
        }
    }
}
