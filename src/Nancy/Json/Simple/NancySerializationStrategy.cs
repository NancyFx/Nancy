namespace Nancy.Json.Simple
{
    using System;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NancySerializationStrategy"/> class.
        /// </summary>
        public NancySerializationStrategy() : this(false)
        {

        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="NancySerializationStrategy"/> class.
        /// </summary>
        /// <param name="retainCasing">Retain C# casing of objects when serialized</param>
        /// <param name="registerConverters">Register Javascript converters</param>
        /// <param name="converters">An array of <see cref="JavaScriptConverter"/></param>
        /// <param name="primitiveConverters">An array of <see cref="JavaScriptPrimitiveConverter"/></param>
        public NancySerializationStrategy(
            bool retainCasing,
            bool registerConverters = true,
            IEnumerable<JavaScriptConverter> converters = null,
            IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters = null)
        {
            this.retainCasing = retainCasing;

            if (registerConverters)
            {
                this.RegisterConverters(converters, primitiveConverters);
            }
        }

        private void RegisterConverters(IEnumerable<JavaScriptConverter> javaScriptConverters, IEnumerable<JavaScriptPrimitiveConverter> javaScriptPrimitiveConverters)
        {
            if (javaScriptConverters != null)
            {
                this.RegisterConverters(javaScriptConverters);
            }

            if (javaScriptPrimitiveConverters != null)
            {
                this.RegisterConverters(javaScriptPrimitiveConverters);
            }
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
        /// <returns></returns>
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
        /// <returns></returns>
        public override object DeserializeObject(object value, Type type)
        {
            if (type.IsEnum || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type).IsEnum))
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
                return base.DeserializeObject(value, type);
            }

            var javascriptConverter = this.FindJavaScriptConverter(type);
            if (javascriptConverter != null)
            {
                return javascriptConverter.Deserialize(valueDictionary, type);
            }

            if (!type.IsGenericType)
            {
                return base.DeserializeObject(value, type);
            }

            var genericType = type.GetGenericTypeDefinition();
            var genericTypeConverter = this.FindJavaScriptConverter(genericType);

            if (genericTypeConverter == null)
            {
                return base.DeserializeObject(value, type);
            }

            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var genericArguments = type.GetGenericArguments();

            for (var i = 0; i < genericArguments.Length; i++)
            {
                var deserializedObject = this.DeserializeObject(valueDictionary.Values.ElementAt(i),
                    genericArguments[i]);

                values.Add(valueDictionary.Keys.ElementAt(i), deserializedObject);
            }

            return genericTypeConverter.Deserialize(values, type);
        }

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="input">The object to serialize</param>
        /// <param name="output">The serialized object</param>
        /// <returns></returns>
        protected override bool TrySerializeKnownTypes(object input, out object output)
        {
            var dynamicValue = input as DynamicDictionaryValue;
            if (!ReferenceEquals(dynamicValue, null) && dynamicValue.HasValue)
            {
                output = dynamicValue.Value;
                return true;
            }

            var inputType = input.GetType().GetTypeInfo();
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
            var primitiveConverter =
                this.primitiveConverters.FirstOrDefault(x => x.SupportedTypes.Any(st => st.IsAssignableFrom(inputType)));

            return primitiveConverter;
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
            var converter = this.converters.FirstOrDefault(x => x.SupportedTypes.Any(st => st.IsAssignableFrom(inputType)));

            return converter;
        }
    }
}
