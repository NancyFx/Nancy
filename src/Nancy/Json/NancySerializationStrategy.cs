namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Extensions;
    using Nancy.Reflection;

    internal class NancySerializationStrategy : PocoJsonSerializerStrategy
    {
        private readonly bool retainCasing;
        private readonly List<JavaScriptConverter> _converters = new List<JavaScriptConverter>();
        private readonly List<JavaScriptPrimitiveConverter> _primitiveConverters = new List<JavaScriptPrimitiveConverter>();


        public NancySerializationStrategy() : this(false) { }

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

        private void RegisterConverters(IEnumerable<JavaScriptConverter> converters, IEnumerable<JavaScriptPrimitiveConverter> primitiveConverters)
        {
            if (converters != null)
            {
                RegisterConverters(converters);
            }
            if (primitiveConverters != null)
            {
                RegisterConverters(primitiveConverters);
            }
        }

        public void RegisterConverters(IEnumerable<JavaScriptConverter> converters)
        {
            _converters.AddRange(converters);
        }

        public void RegisterConverters(IEnumerable<JavaScriptPrimitiveConverter> converters)
        {
            _primitiveConverters.AddRange(converters);
        }

        protected override string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return retainCasing ? base.MapClrMemberNameToJsonFieldName(clrPropertyName) :
                clrPropertyName.ToCamelCase();
        }

        internal override ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
        {
            return base.ContructorDelegateFactory(key);
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

        internal override System.Collections.Generic.IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            return base.GetterValueFactory(type);
        }

        protected override object SerializeEnum(Enum p)
        {
            return base.SerializeEnum(p);
        }

        internal override System.Collections.Generic.IDictionary<string, System.Collections.Generic.KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            return base.SetterValueFactory(type);
        }

        protected override bool TrySerializeKnownTypes(object input, out object output)
        {
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

        public override bool TrySerializeNonPrimitiveObject(object input, out object output)
        {
            return base.TrySerializeNonPrimitiveObject(input, out output);
        }

        protected override bool TrySerializeUnknownTypes(object input, out object output)
        {
            return base.TrySerializeUnknownTypes(input, out output);
        }
    }
}
