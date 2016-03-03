namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;

    public abstract class JavaScriptPrimitiveConverter
    {
        public abstract IEnumerable<Type> SupportedTypes { get; }

        public virtual object Deserialize(object primitiveValue, Type type)
        {
            return Deserialize(primitiveValue, type, null);    
        }

        public abstract object Deserialize(object primitiveValue, Type type, JavaScriptSerializer serializer);

        public virtual object Serialize(object obj)
        {
            return Serialize(obj, null);
        }

        public abstract object Serialize(object obj, JavaScriptSerializer serializer);
    }
}
