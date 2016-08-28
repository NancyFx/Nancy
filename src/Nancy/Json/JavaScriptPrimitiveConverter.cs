namespace Nancy.Json
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  Operations for converting javascript primitives.
    /// </summary>
    public abstract class JavaScriptPrimitiveConverter
    {
        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <value>
        /// The supported types.
        /// </value>
        public abstract IEnumerable<Type> SupportedTypes { get; }

        /// <summary>
        /// Deserializes the specified primitive value.
        /// </summary>
        /// <param name="primitiveValue">The primitive value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual object Deserialize(object primitiveValue, Type type)
        {
            return Deserialize(primitiveValue, type, null);    
        }

        /// <summary>
        /// Deserializes the specified primitive value.
        /// </summary>
        /// <param name="primitiveValue">The primitive value.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public abstract object Deserialize(object primitiveValue, Type type, JavaScriptSerializer serializer);

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public virtual object Serialize(object obj)
        {
            return Serialize(obj, null);
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public abstract object Serialize(object obj, JavaScriptSerializer serializer);
    }
}
