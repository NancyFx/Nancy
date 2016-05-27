namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Reflection;

    /// <summary>
    /// Contains extensions to <see cref="object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Convert an object to a dynamic type
        /// </summary>
        /// <param name="value">An object to convert to dynamic</param>
        /// <returns>Returns a dynamic version of the specified type</returns>
        public static dynamic ToDynamic(this object value)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in value.GetType().GetTypeInfo().DeclaredProperties)
            {
                expando.Add(property.Name, property.GetValue(value));
            }

            return (ExpandoObject)expando;
        }
    }
}