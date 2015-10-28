namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;

    public abstract class JavaScriptPrimitiveConverter
    {
        public abstract IEnumerable<Type> SupportedTypes { get; }

        public abstract object Deserialize(object primitiveValue, Type type, JavaScriptSerializer serializer);
        public abstract object Serialize(object obj, JavaScriptSerializer serializer);
    }
}
