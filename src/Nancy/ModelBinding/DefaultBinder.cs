namespace Nancy.ModelBinding
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

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

        private readonly static MethodInfo ToListMethodInfo = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
        private readonly static MethodInfo ToArrayMethodInfo = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
        private static readonly Regex BracketRegex = new Regex(@"\[(\d+)\]\z", RegexOptions.Compiled);
        private static readonly Regex UnderscoreRegex = new Regex(@"_(\d+)\z", RegexOptions.Compiled);

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
        /// <param name="blackList">Blacklisted binding property names</param>
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

            try
            {
                var bodyDeserializedModel = this.DeserializeRequestBody(bindingContext);
                if (bodyDeserializedModel != null)
                {
                    UpdateModelWithDeserializedModel(bodyDeserializedModel, bindingContext);
                }
            }
            catch (Exception exception)
            {
                if (!bindingContext.Configuration.IgnoreErrors)
                {
                    throw new ModelBindingException(modelType, innerException: exception);
                }
            }

            var bindingExceptions = new List<PropertyBindingException>();

            if (!bindingContext.Configuration.BodyOnly)
            {
                if (bindingContext.DestinationType.IsCollection() || bindingContext.DestinationType.IsArray() ||bindingContext.DestinationType.IsEnumerable())
                {
                    var loopCount = this.GetBindingListInstanceCount(context);
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

                        foreach (var modelProperty in bindingContext.ValidModelBindingMembers)
                        {
                            var existingCollectionValue = modelProperty.GetValue(genericinstance);

                            var collectionStringValue = GetValue(modelProperty.Name, bindingContext, i);

                            if (this.BindingValueIsValid(collectionStringValue, existingCollectionValue, modelProperty,
                                                    bindingContext))
                            {
                                try
                                {
                                    BindValue(modelProperty, collectionStringValue, bindingContext, genericinstance);
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
                    foreach (var modelProperty in bindingContext.ValidModelBindingMembers)
                    {
                        var existingValue = modelProperty.GetValue(bindingContext.Model);

                        var stringValue = GetValue(modelProperty.Name, bindingContext);

                        if (this.BindingValueIsValid(stringValue, existingValue, modelProperty, bindingContext))
                        {
                            try
                            {
                                BindValue(modelProperty, stringValue, bindingContext);
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
                var generictoArrayMethod = ToArrayMethodInfo.MakeGenericMethod(new[] { genericType });
                return generictoArrayMethod.Invoke(null, new[] { bindingContext.Model });
            }
            return bindingContext.Model;
        }

        private bool BindingValueIsValid(string bindingValue, object existingValue, BindingMemberInfo modelProperty, BindingContext bindingContext)
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
            var bracketMatch = BracketRegex.Match(item);
            if (bracketMatch.Success)
            {
                return int.Parse(bracketMatch.Groups[1].Value);
            }

            var underscoreMatch = UnderscoreRegex.Match(item);

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

                    if (o.GetType().IsValueType || o is string)
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
                foreach (var modelProperty in bindingContext.ValidModelBindingMembers)
                {
                    var existingValue =
                        modelProperty.GetValue(bindingContext.Model);

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

            foreach (var modelProperty in bindingContext.ValidModelBindingMembers)
            {
                var existingValue = modelProperty.GetValue(genericTypeInstance);

                if (IsDefaultValue(existingValue, modelProperty.PropertyType) || bindingContext.Configuration.Overwrite)
                {
                    CopyValue(modelProperty, o, genericTypeInstance);
                }
            }
        }

        private static void CopyValue(BindingMemberInfo modelProperty, object source, object destination)
        {
            var newValue = modelProperty.GetValue(source);

            modelProperty.SetValue(destination, newValue);
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
                ValidModelBindingMembers = GetBindingMembers(modelType, genericType, blackList).ToList(),
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

        private static void BindValue(BindingMemberInfo modelProperty, string stringValue, BindingContext context)
        {
            BindValue(modelProperty, stringValue, context, context.Model);
        }

        private static void BindValue(BindingMemberInfo modelProperty, string stringValue, BindingContext context, object targetInstance)
        {
            var destinationType = modelProperty.PropertyType;

            var typeConverter =
                context.TypeConverters.FirstOrDefault(c => c.CanConvertTo(destinationType, context));

            if (typeConverter != null)
            {
                try
                {
                    SetBindingMemberValue(modelProperty, targetInstance, typeConverter.Convert(stringValue, destinationType, context));
                }
                catch (Exception e)
                {
                    throw new PropertyBindingException(modelProperty.Name, stringValue, e);
                }
            }
            else if (destinationType == typeof(string))
            {
                SetBindingMemberValue(modelProperty, targetInstance, stringValue);
            }
        }

        private static void SetBindingMemberValue(BindingMemberInfo modelProperty, object model, object value)
        {
            // TODO - catch reflection exceptions?
            modelProperty.SetValue(model, value);
        }

        private static IEnumerable<BindingMemberInfo> GetBindingMembers(Type modelType, Type genericType, IEnumerable<string> blackList)
        {
            var blackListHash = new HashSet<string>(blackList, StringComparer.Ordinal);

            return BindingMemberInfo.Collect(genericType ?? modelType)
                .Where(member => !blackListHash.Contains(member.Name));
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
                    var genericMethod = ToListMethodInfo.MakeGenericMethod(genericType);
                    return genericMethod.Invoke(null, new[] { instance });
                }

                //else just make a list
                var listType = typeof(List<>).MakeGenericType(genericType);
                return Activator.CreateInstance(listType);
            }

            if (instance == null)
            {
                return Activator.CreateInstance(modelType, true);
            }

            return !modelType.IsInstanceOfType(instance) ?
                Activator.CreateInstance(modelType, true) :
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
                            return c.Key.StartsWith(propertyName, StringComparison.OrdinalIgnoreCase) && indexId != -1 && indexId == indexindexes[index];
                        })
                        .Select(k => k.Value)
                        .FirstOrDefault();

                    return propertyValue ?? string.Empty;
                }

                return string.Empty;
            }
            return context.RequestData.ContainsKey(propertyName) ? context.RequestData[propertyName] : string.Empty;
        }

        private object DeserializeRequestBody(BindingContext context)
        {
            if (context.Context == null || context.Context.Request == null)
            {
                return null;
            }

            var contentType = GetRequestContentType(context.Context);

            var bodyDeserializer = this.bodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType, context))
                ?? this.defaults.DefaultBodyDeserializers.FirstOrDefault(b => b.CanDeserialize(contentType, context));

            return bodyDeserializer != null
                ? bodyDeserializer.Deserialize(contentType, context.Context.Request.Body, context)
                : null;
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
