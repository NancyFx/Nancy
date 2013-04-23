namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Nancy.Extensions;
    using System.Collections;

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

        private readonly static MethodInfo toListMethodInfo = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
        private readonly static MethodInfo toArrayMethodInfo = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);

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
            Type genericType = null;
            if (modelType.IsArray() || modelType.IsCollection() || modelType.IsEnumerable())
            {
                //make sure it has a generic type
                if (modelType.IsGenericType())
                {
                    genericType = modelType.GetGenericArguments().FirstOrDefault();
                }
                else
                {
                    var ienumerable =
                        modelType.GetInterfaces().Where(i => i.IsGenericType()).FirstOrDefault(
                            i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    genericType = ienumerable == null ? null : ienumerable.GetGenericArguments().FirstOrDefault();
                }

                if (genericType == null)
                {
                    throw new ArgumentException("when modeltype is an enumerble it must specify the type", "modelType");
                }
            }

            var bindingContext =
                this.CreateBindingContext(context, modelType, instance, configuration, blackList, genericType);

            var bodyDeserializedModel = this.DeserializeRequestBody(bindingContext);

            if (bodyDeserializedModel != null)
            {
                UpdateModelWithDeserializedModel(bodyDeserializedModel, bindingContext);
            }

            if (!configuration.BodyOnly)
            {
                var bindingExceptions = new List<PropertyBindingException>();

                if (bindingContext.Model.GetType().IsCollection())
                {
                    var loopCount = GetBindingListInstanceCount(context);

                    for (int i = 1; i <= loopCount; i++)
                    {
                        var genericinstance = Activator.CreateInstance(genericType);

                        foreach (var modelProperty in bindingContext.ValidModelProperties)
                        {
                            var existingCollectionValue = modelProperty.GetValue(genericinstance, null);

                            var collectionStringValue = GetValue(modelProperty.Name + "_" + i, bindingContext);

                            if (BindingValueIsValid(collectionStringValue, existingCollectionValue, modelProperty, bindingContext))
                            {
                                try
                                {
                                    BindProperty(modelProperty, collectionStringValue, bindingContext, genericinstance);
                                }
                                catch (PropertyBindingException ex)
                                {
                                    bindingExceptions.Add(ex);
                                }
                            }
                        }

                        var list = bindingContext.Model as IList;
                        list.Add(genericinstance);
                    }
                }
                else
                {
                    foreach (var modelProperty in bindingContext.ValidModelProperties)
                    {
                        var existingValue = modelProperty.GetValue(bindingContext.Model, null);

                        var stringValue = GetValue(modelProperty.Name, bindingContext);

                        if (BindingValueIsValid(stringValue, existingValue, modelProperty, bindingContext))
                        {
                            try
                            {
                                BindProperty(modelProperty, stringValue, bindingContext);
                            }
                            catch (PropertyBindingException ex)
                            {
                                bindingExceptions.Add(ex);
                            }
                        }
                    }
                }

                if (bindingExceptions.Any())
                {
                    throw new ModelBindingException(modelType, bindingExceptions);
                }
            }

            if (modelType.IsArray())
            {
                var generictoArrayMethod = toArrayMethodInfo.MakeGenericMethod(new[] { genericType });
                return generictoArrayMethod.Invoke(null, new[] { bindingContext.Model });
            }
            return bindingContext.Model;
        }

        private bool BindingValueIsValid(string bindingValue, object existingValue, PropertyInfo modelProperty, BindingContext bindingContext)
        {
            return (!String.IsNullOrEmpty(bindingValue) &&
                    (IsDefaultValue(existingValue, modelProperty.PropertyType) ||
                     bindingContext.Configuration.Overwrite));
        }

        private int GetBindingListInstanceCount(NancyContext context)
        {
            var dictionary = context.Request.Form as IDictionary<string, object>;
            if (dictionary == null)
            {
                return 0;
            }

            var matches = dictionary.Keys.Where(x => IsMatch(x)).ToArray();

            if (!matches.Any())
            {
                return 0;
            }

            var orderedFormParam = matches.OrderByDescending(y => y).First();

            var value = int.Parse(orderedFormParam[orderedFormParam.Length - 1].ToString());

            return value;
        }

        private bool IsMatch(string item)
        {
            return item.Length >= 2 && item[item.Length - 2] == '_' && Char.IsDigit(item[item.Length - 1]);
        }

        private static void UpdateModelWithDeserializedModel(object bodyDeserializedModel, BindingContext bindingContext)
        {
            if (bodyDeserializedModel.GetType().IsCollection() || bodyDeserializedModel.GetType().IsEnumerable() ||
                bodyDeserializedModel.GetType().IsArray())
            {
                var enumerabe = (IEnumerable)bodyDeserializedModel;
                foreach (var o in enumerabe)
                {
                    var genericTypeInstance = Activator.CreateInstance(bindingContext.GenericType);
                    foreach (var modelProperty in bindingContext.ValidModelProperties)
                    {
                        var existingValue =
                            modelProperty.GetValue(genericTypeInstance, null);

                        if (IsDefaultValue(existingValue, modelProperty.PropertyType) ||
                            bindingContext.Configuration.Overwrite)
                        {
                            CopyValue(modelProperty, o, genericTypeInstance);
                        }
                    }
                    ((IList)bindingContext.Model).Add(genericTypeInstance);
                }
            }
            else
            {
                foreach (var modelProperty in bindingContext.ValidModelProperties)
                {
                    var existingValue =
                        modelProperty.GetValue(bindingContext.Model, null);

                    if (IsDefaultValue(existingValue, modelProperty.PropertyType) || bindingContext.Configuration.Overwrite)
                    {
                        CopyValue(modelProperty, bodyDeserializedModel, bindingContext.Model);
                    }
                }
            }
        }

        private static void CopyValue(PropertyInfo modelProperty, object bodyDeserializedModel, object model)
        {
            var newValue = modelProperty.GetValue(bodyDeserializedModel, null);

            modelProperty.SetValue(model, newValue, null);
        }

        private static bool IsDefaultValue(object existingValue, Type propertyType)
        {
            return propertyType.IsValueType
                ? Equals(existingValue, Activator.CreateInstance(propertyType))
                : existingValue == null;
        }

        private BindingContext CreateBindingContext(NancyContext context, Type modelType, object instance, BindingConfig configuration, IEnumerable<string> blackList, Type genericType)
        {
            return new BindingContext
            {
                Configuration = configuration,
                Context = context,
                DestinationType = modelType,
                Model = CreateModel(modelType, genericType, instance),
                ValidModelProperties = GetProperties(modelType, genericType, blackList),
                RequestData = this.GetDataFields(context),
                GenericType = genericType,
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
                catch (Exception e)
                {
                    throw new PropertyBindingException(modelProperty.Name, stringValue, e);
                }
            }
            else if (destinationType == typeof(string))
            {
                SetPropertyValue(modelProperty, context.Model, stringValue);
            }
        }

        private static void BindProperty(PropertyInfo modelProperty, string stringValue, BindingContext context, object genericInstance)
        {
            var destinationType = modelProperty.PropertyType;

            var typeConverter =
                context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(destinationType, context));

            if (typeConverter != null)
            {
                try
                {
                    SetPropertyValue(modelProperty, genericInstance, typeConverter.Convert(stringValue, destinationType, context));
                }
                catch (Exception e)
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

        private static IEnumerable<PropertyInfo> GetProperties(Type modelType, Type genericType, IEnumerable<string> blackList)
        {
            if (genericType != null)
            {
                return genericType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && !blackList.Contains(p.Name, StringComparer.InvariantCulture))
                .Where(property => !property.GetIndexParameters().Any());
            }
            else
            {
                return modelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && !blackList.Contains(p.Name, StringComparer.InvariantCulture))
                .Where(property => !property.GetIndexParameters().Any());
            }
        }

        private static object CreateModel(Type modelType, Type genericType, object instance)
        {
            if (modelType.IsArray() || modelType.IsCollection() || modelType.IsEnumerable())
            {
                //make sure instance has a Add method. Otherwise call `.ToList`
                if (instance != null && modelType.IsInstanceOfType(instance))
                {
                    var addMethod = modelType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);
                    if (addMethod != null)
                    {
                        return instance;
                    }
                    return toListMethodInfo.Invoke(null, new[] { instance });
                }

                //else just make a list
                var listType = typeof(List<>).MakeGenericType(genericType);
                return Activator.CreateInstance(listType);
            }

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