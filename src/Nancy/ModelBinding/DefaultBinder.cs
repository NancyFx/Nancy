namespace Nancy.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

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
        private static readonly Regex bracketRegex = new Regex(@"\[(\d+)\]\z", RegexOptions.Compiled);
        private static readonly Regex underscoreRegex = new Regex(@"_(\d+)\z", RegexOptions.Compiled);

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
                    throw new ArgumentException("When modelType is an enumerable it must specify the type.", "modelType");
                }
            }

            var bindingContext =
                this.CreateBindingContext(context, modelType, instance, configuration, blackList, genericType);

            var bodyDeserializedModel = this.DeserializeRequestBody(bindingContext);

            if (bodyDeserializedModel != null)
            {
                UpdateModelWithDeserializedModel(bodyDeserializedModel, bindingContext);
            }

            var bindingExceptions = new List<PropertyBindingException>();

            if (!bindingContext.Configuration.BodyOnly)
            {
                if (bindingContext.DestinationType.IsCollection() || bindingContext.DestinationType.IsArray() ||bindingContext.DestinationType.IsEnumerable())
                {
                    var loopCount = GetBindingListInstanceCount(context);
                    var model = (IList)bindingContext.Model;
                    for (var i = 0; i < loopCount; i++)
                    {
                        object genericinstance;
                        if (model.Count > i)
                        {
                            genericinstance = model[i];
                        }
                        else
                        {
                            genericinstance = Activator.CreateInstance(bindingContext.GenericType);
                            model.Add(genericinstance);
                        }

                        foreach (var modelProperty in bindingContext.ValidModelProperties)
                        {
                            var existingCollectionValue = modelProperty.GetValue(genericinstance, null);

                            var collectionStringValue = GetValue(modelProperty.Name, bindingContext, i);

                            if (BindingValueIsValid(collectionStringValue, existingCollectionValue, modelProperty,
                                                    bindingContext))
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

                if (bindingExceptions.Any() && !bindingContext.Configuration.IgnoreErrors)
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

        /// <summary>
        /// Gets the number of distinct indexes from context:
        ///
        /// i.e:
        ///  IntProperty_5
        ///  StringProperty_5
        ///  IntProperty_7
        ///  StringProperty_8
        ///  You'll end up with a list of 3 matches: 5,7,8
        /// 
        /// </summary>
        /// <param name="context">Current Context </param>
        /// <returns>An int containing the number of elements</returns>
        private int GetBindingListInstanceCount(NancyContext context)
        {
            var dictionary = context.Request.Form as IDictionary<string, object>;
            
            if (dictionary == null)
            {
                return 0;
            }

            return dictionary.Keys.Select(IsMatch).Where(x => x != -1).Distinct().ToArray().Length;
        }

        private static int IsMatch(string item)
        {
            var bracketMatch = bracketRegex.Match(item);
            if (bracketMatch.Success)
            {
                return int.Parse(bracketMatch.Groups[1].Value);
            }

            var underscoreMatch = underscoreRegex.Match(item);

            if (underscoreMatch.Success)
            {
                return int.Parse(underscoreMatch.Groups[1].Value);
            }

            return -1;
        }

        private static void UpdateModelWithDeserializedModel(object bodyDeserializedModel, BindingContext bindingContext)
        {
            var bodyDeserializedModelType = bodyDeserializedModel.GetType();

            if (bodyDeserializedModelType.IsValueType)
            {
                bindingContext.Model = bodyDeserializedModel;
                return;
            }

            if (bodyDeserializedModelType.IsCollection() || bodyDeserializedModelType.IsEnumerable() ||
                bodyDeserializedModelType.IsArray())
            {
                var count = 0;

                foreach (var o in (IEnumerable)bodyDeserializedModel)
                {
                    var model = (IList)bindingContext.Model;

                    if (o.GetType().IsValueType || o.GetType() == typeof(string))
                    {
                        HandleValueTypeCollectionElement(model, count, o);
                    }
                    else
                    {
                        HandleReferenceTypeCollectionElement(bindingContext, model, count, o);
                    }

                    count++;
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

        private static void HandleValueTypeCollectionElement(IList model, int count, object o)
        {
            // If the instance specified in the binder contains the n-th element use that 
            if (model.Count > count)
            {
                return;
            }

            model.Add(o);
        }

        private static void HandleReferenceTypeCollectionElement(BindingContext bindingContext, IList model, int count, object o)
        {
            // If the instance specified in the binder contains the n-th element use that otherwise make a new one.
            object genericTypeInstance;
            if (model.Count > count)
            {
                genericTypeInstance = model[count];
            }
            else
            {
                genericTypeInstance = Activator.CreateInstance(bindingContext.GenericType);
                model.Add(genericTypeInstance);
            }

            foreach (var modelProperty in bindingContext.ValidModelProperties)
            {
                var existingValue = modelProperty.GetValue(genericTypeInstance, null);

                if (IsDefaultValue(existingValue, modelProperty.PropertyType) || bindingContext.Configuration.Overwrite)
                {
                    CopyValue(modelProperty, o, genericTypeInstance);
                }
            }
        }

        private static void CopyValue(PropertyInfo modelProperty, object source, object destination)
        {
            var newValue = modelProperty.GetValue(source, null);

            modelProperty.SetValue(destination, newValue, null);
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
                    var genericMethod = toListMethodInfo.MakeGenericMethod(genericType);
                    return genericMethod.Invoke(null, new[] { instance });
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

        private static string GetValue(string propertyName, BindingContext context, int index = -1)
        {
            if (index != -1)
            {

                var indexindexes = context.RequestData.Keys.Select(IsMatch)
                                           .Where(i => i != -1)
                                           .OrderBy(i => i)
                                           .Distinct()
                                           .Select((k, i) => new KeyValuePair<int, int>(i, k))
                                           .ToDictionary(k => k.Key, v => v.Value);

                if (indexindexes.ContainsKey(index))
                {
                    var propertyValue =
                        context.RequestData.Where(c =>
                        {
                            var indexId = IsMatch(c.Key);
                            return c.Key.StartsWith(propertyName) && indexId != -1 && indexId == indexindexes[index];
                        })
                        .Select(k => k.Value)
                        .FirstOrDefault();

                    return propertyValue ?? string.Empty;
                }
                
                return String.Empty;
            }
            return context.RequestData.ContainsKey(propertyName) ? context.RequestData[propertyName] : String.Empty;
        }

        private object DeserializeRequestBody(BindingContext context)
        {
            if (context.Context == null || context.Context.Request == null)
            {
                return null;
            }

            var contentType = GetRequestContentType(context.Context);
            var bodyDeserializer = this.bodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType, context));

            if (bodyDeserializer != null)
            {
                return bodyDeserializer.Deserialize(contentType, context.Context.Request.Body, context);
            }

            bodyDeserializer = this.defaults.DefaultBodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType, context));

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
