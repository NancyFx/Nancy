namespace Nancy.Json.Simple
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Nancy.Extensions;

    /// <summary>
    /// Nancy serialization stategy for SimpleJson
    /// </summary>
    public class NancySerializationStrategy : PocoJsonSerializerStrategy
    {
        private readonly bool retainCasing;
        private readonly List<JavaScriptConverter> converters = new List<JavaScriptConverter>();
        private readonly List<JavaScriptPrimitiveConverter> primitiveConverters = new List<JavaScriptPrimitiveConverter>();
        private readonly ConcurrentDictionary<Type, JavaScriptConverter> converterCache = new ConcurrentDictionary<Type, JavaScriptConverter>();
        private readonly ConcurrentDictionary<Type, JavaScriptPrimitiveConverter> primitiveConverterCache = new ConcurrentDictionary<Type, JavaScriptPrimitiveConverter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NancySerializationStrategy"/> class.
        /// </summary>
        /// <remarks>C# casing of objects will be defaulted to camelCase</remarks>
        public NancySerializationStrategy() : this(false)
        {

        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="NancySerializationStrategy"/> class.
        /// </summary>
        /// <param name="retainCasing">Retain C# casing of objects when serialized</param>
        public NancySerializationStrategy(bool retainCasing)
        {
            this.retainCasing = retainCasing;
        }

        /// <summary>
        /// Register custom <see cref="JavaScriptConverter"/> converters
        /// </summary>
        /// <param name="javaScriptConverters">An array of <see cref="JavaScriptConverter"/></param>
        public void RegisterConverters(IEnumerable<JavaScriptConverter> javaScriptConverters)
        {
            this.converters.AddRange(javaScriptConverters);
        }

        /// <summary>
        /// Register custom <see cref="JavaScriptPrimitiveConverter"/>
        /// </summary>
        /// <param name="javaScriptPrimitiveConverters">An array of <see cref="JavaScriptPrimitiveConverter"/></param>
        public void RegisterConverters(IEnumerable<JavaScriptPrimitiveConverter> javaScriptPrimitiveConverters)
        {
            this.primitiveConverters.AddRange(javaScriptPrimitiveConverters);
        }

        /// <summary>
        /// Formats a property name to a JSON field name
        /// </summary>
        /// <param name="clrPropertyName">The property name to format</param>
        /// <returns>camelCase <paramref name="clrPropertyName"/> if retainCasing is false, otherwise <paramref name="clrPropertyName"/></returns>
        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return this.retainCasing
                ? base.MapClrMemberNameToJsonFieldName(clrPropertyName)
                : clrPropertyName.ToCamelCase();
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <param name="value">The object to deserialize</param>
        /// <param name="type">The type of object to deserialize</param>
        /// <param name="dateTimeStyles">The <see cref="DateTimeStyles"/> ton convert <see cref="DateTime"/> objects</param>
        /// <returns>A instance of <paramref name="type" /> deserialized from <paramref name="value"/></returns>
        public override object DeserializeObject(object value, Type type, DateTimeStyles dateTimeStyles)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type).GetTypeInfo().IsEnum))
            {
                var typeToParse = ReflectionUtils.IsNullableType(type)
                    ? Nullable.GetUnderlyingType(type)
                    : type;

                return value == null
                    ? null
                    : Enum.Parse(typeToParse, value.ToString(), true);
            }

            var primitiveConverter = this.FindPrimitiveConverter(type);
            if (primitiveConverter != null)
            {
                return primitiveConverter.Deserialize(value, type);
            }

            var valueDictionary = value as IDictionary<string, object>;
            if (valueDictionary == null)
            {
                return base.DeserializeObject(value, type, dateTimeStyles);
            }

            var javascriptConverter = this.FindJavaScriptConverter(type);
            if (javascriptConverter != null)
            {
                return javascriptConverter.Deserialize(valueDictionary, type);
            }

            if (!typeInfo.IsGenericType)
            {
                return base.DeserializeObject(value, type, dateTimeStyles);
            }

            var genericType = typeInfo.GetGenericTypeDefinition();
            var genericTypeConverter = this.FindJavaScriptConverter(genericType);

            if (genericTypeConverter == null)
            {
                return base.DeserializeObject(value, type, dateTimeStyles);
            }

            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var genericArguments = type.GetGenericArguments();

            for (var i = 0; i < genericArguments.Length; i++)
            {
                var deserializedObject = this.DeserializeObject(valueDictionary.Values.ElementAt(i),
                    genericArguments[i], dateTimeStyles);

                values.Add(valueDictionary.Keys.ElementAt(i), deserializedObject);
            }

            return genericTypeConverter.Deserialize(values, type);
        }

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="input">The object to serialize</param>
        /// <param name="output">The serialized object</param>
        /// <returns>true if <paramref name="input"/> was converted successfully; otherwise, false</returns>
        protected override bool TrySerializeKnownTypes(object input, out object output)
        {
            var dynamicValue = input as DynamicDictionaryValue;
            if (!ReferenceEquals(dynamicValue, null) && dynamicValue.HasValue)
            {
                output = dynamicValue.Value;
                return true;
            }

            var inputType = input.GetType();

            if (this.TrySerializeJavaScriptConverter(input, out output, inputType))
            {
                return true;
            }

            if (this.TrySerializePrimitiveConverter(input, ref output, inputType))
            {
                return true;
            }

            var type = input as Type;

            if (type != null)
            {
                output = type.GetTypeInfo().FullName;
                return true;
            }

            if (input is DateTime)
            {
                return this.SerializeDateTime((DateTime)input, out output);
            }

            if (input is DateTimeOffset)
            {
                var dto = (DateTimeOffset)input;
                output = dto.ToString("o", CultureInfo.InvariantCulture);
                return true;
            }

            return base.TrySerializeKnownTypes(input, out output);
        }

        private bool SerializeDateTime(DateTime input, out object output)
        {
            var dateTime = input;
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Local);
            }

            output = dateTime.ToString("o", CultureInfo.InvariantCulture);

            return true;
        }

        private bool TrySerializePrimitiveConverter(object input, ref object output, Type inputType)
        {
            var primitiveConverter = this.FindPrimitiveConverter(inputType);
            if (primitiveConverter == null)
            {
                return false;
            }

            output = primitiveConverter.Serialize(input);
            return true;
        }

        private JavaScriptPrimitiveConverter FindPrimitiveConverter(Type inputType)
        {
            return this.primitiveConverterCache.GetOrAdd(inputType, typeToConvert => this.primitiveConverters.FirstOrDefault(converter => converter.SupportedTypes.Any(supportedType => supportedType.IsAssignableFrom(typeToConvert))));
        }

        private bool TrySerializeJavaScriptConverter(object input, out object output, Type inputType)
        {
            output = null;
            var converter = this.FindJavaScriptConverter(inputType);
            if (converter == null)
            {
                return false;
            }
            var result = converter.Serialize(input);
            output = result.ToDictionary(kvp => this.MapClrMemberNameToJsonFieldName(kvp.Key), kvp => kvp.Value);
            return true;
        }

        private JavaScriptConverter FindJavaScriptConverter(Type inputType)
        {
            return this.converterCache.GetOrAdd(inputType, typeToConvert => this.converters.FirstOrDefault(converter => converter.SupportedTypes.Any(supportedType => supportedType.IsAssignableFrom(typeToConvert))));
        }
    }
}
