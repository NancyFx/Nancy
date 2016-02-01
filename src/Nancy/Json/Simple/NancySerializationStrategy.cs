namespace Nancy.Json.Simple
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Nancy.Extensions;

    internal class NancySerializationStrategy : PocoJsonSerializerStrategy
    {
        private readonly bool retainCasing;

        private readonly bool useIso8601;

        private readonly List<JavaScriptConverter> _converters = new List<JavaScriptConverter>();
        private readonly List<JavaScriptPrimitiveConverter> _primitiveConverters = new List<JavaScriptPrimitiveConverter>();
        internal static readonly long InitialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        static readonly DateTime MinimumJavaScriptDate = new DateTime(100, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public NancySerializationStrategy() : this(false) { }

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

        private void RegisterConverters(IEnumerable<JavaScriptConverter> converters, IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (converters != null)
            {
                this.RegisterConverters(converters);
            }
            if (primitiveConverters != null)
            {
                this.RegisterConverters(primitiveConverters);
            }
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            this._converters.AddRange(converters);
        }

        public void RegisterConverters(IEnumerable<JavaScriptPrimitiveConverter> converters)
        {
            this._primitiveConverters.AddRange(converters);
        }

        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return this.retainCasing ? base.MapClrMemberNameToJsonFieldName(clrPropertyName) :
                clrPropertyName.ToCamelCase();
        }

        public override object DeserializeObject(object value, Type type)
        {
            if (type.IsEnum || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type).IsEnum))									
                return value == null ? null : Enum.Parse(ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type, value.ToString(), true);

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
                        for (int i = 0; i < genericArguments.Length; i++)
                        {
                            values.Add(valueDictionary.Keys.ElementAt(i), this.DeserializeObject(valueDictionary.Values.ElementAt(i), genericArguments[i]));
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
            if (this.TrySerializeJavaScriptConverter(input, out output, inputType)) return true;

            if (this.TrySerializePrimitiveConverter(input, ref output, inputType)) return true;
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
                    time = MinimumJavaScriptDate;

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
                this._primitiveConverters.FirstOrDefault(x => x.SupportedTypes.Any(st => st.IsAssignableFrom(inputType)));
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
            var converter = this._converters.FirstOrDefault(x => x.SupportedTypes.Any(st => st.IsAssignableFrom(inputType)));
            return converter;
        }
    }
}
