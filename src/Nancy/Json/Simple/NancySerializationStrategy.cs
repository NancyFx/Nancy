namespace Nancy.Json.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Nancy.Extensions;

    /// <summary>
    /// Nancy serialization stategy for SimpleJson
    /// </summary>
    public class NancySerializationStrategy : PocoJsonSerializerStrategy
    {
        private readonly bool retainCasing;
        private readonly bool useIso8601;
        private readonly List<JavaScriptConverter> converters = new List<JavaScriptConverter>();
        private readonly List<JavaScriptPrimitiveConverter> primitiveConverters = new List<JavaScriptPrimitiveConverter>();
        private static readonly long InitialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly DateTime MinimumJavaScriptDate = new DateTime(100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
        /// <param name="useIso8601">Use ISO8601 format when serializing date objects</param>
        /// <param name="converters">An array of <see cref="JavaScriptConverter"/></param>
        /// <param name="primitiveConverters">An array of <see cref="JavaScriptPrimitiveConverter"/></param>
        public NancySerializationStrategy(
            bool retainCasing,
            bool registerConverters = true,
            bool useIso8601 = true,
            IEnumerable<JavaScriptConverter> converters = null,
            IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters = null)
        {
            this.retainCasing = retainCasing;
            this.useIso8601 = useIso8601;

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
                var typeToParse = ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;

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
            if (valueDictionary != null)
            {
                var javascriptConverter = this.FindJavaScriptConverter(type);
                if (javascriptConverter != null)
                {
                    return javascriptConverter.Deserialize(valueDictionary, type);
                }

                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();
                    var genericTypeConverter = this.FindJavaScriptConverter(genericType);

                    if (genericTypeConverter != null)
                    {
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
                }
            }

            return base.DeserializeObject(value, type);
        }

        protected override bool TrySerializeKnownTypes(object input, out object output)
        {
            var type = input as Type;
            if (type != null)
            {
                output = type.FullName;
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

            return base.TrySerializeKnownTypes(input, out output);
        }

        private bool SerializeDateTime(DateTime input, out object output)
        {
            if (this.useIso8601)
            {
                var dateTime = input;
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Local);
                }

                output = dateTime.ToString("o", CultureInfo.InvariantCulture);
            }
            else
            {
                DateTime time = input.ToUniversalTime();

                string suffix = "";
                if (input.Kind != DateTimeKind.Utc)
                {
                    TimeSpan localTzOffset;
                    if (input >= time)
                    {
                        localTzOffset = input - time;
                        suffix = "+";
                    }
                    else
                    {
                        localTzOffset = time - input;
                        suffix = "-";
                    }
                    suffix += localTzOffset.ToString("hhmm");
                }

                if (time < MinimumJavaScriptDate)
                {
                    time = MinimumJavaScriptDate;
                }

                var ticks = (time.Ticks - InitialJavaScriptDateTicks) / 10000;
                output = "\\/Date(" + ticks + suffix + ")\\/";
            }
            return true;
        }

        private bool TrySerializePrimitiveConverter(object input, ref object output, Type inputType)
        {
            var primitiveConverter = this.FindPrimitiveConverter(inputType);
            if (primitiveConverter != null)
            {
                output = primitiveConverter.Serialize(input);
                return true;
            }

            return false;
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
            if (converter != null)
            {
                var result = converter.Serialize(input);
                output = result.ToDictionary(kvp => this.MapClrMemberNameToJsonFieldName(kvp.Key), kvp => kvp.Value);
                return true;
            }

            return false;
        }

        private JavaScriptConverter FindJavaScriptConverter(Type inputType)
        {
            var converter = this.converters.FirstOrDefault(x => x.SupportedTypes.Any(st => st.IsAssignableFrom(inputType)));

            return converter;
        }
    }
}
