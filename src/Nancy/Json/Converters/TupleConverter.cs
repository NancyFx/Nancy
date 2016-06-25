namespace Nancy.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Converts a dictionary into a list of tuples.
    /// </summary>
    /// <seealso cref="Nancy.Json.JavaScriptConverter" />
    public class TupleConverter : JavaScriptConverter
    {
        /// <summary>
        /// Gets the supported tuple types.
        /// </summary>
        /// <value>
        /// The supported types.
        /// </value>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(Tuple<>);
                yield return typeof(Tuple<,>);
                yield return typeof(Tuple<,,>);
                yield return typeof(Tuple<,,,>);
                yield return typeof(Tuple<,,,,>);
                yield return typeof(Tuple<,,,,,>);
                yield return typeof(Tuple<,,,,,,>);
                yield return typeof(Tuple<,,,,,,,>);
            }
        }

        /// <summary>
        /// Deserializes the specified dictionary into a list of tuples.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var ctor = type.GetConstructors().First();
            object instance = ctor.Invoke(dictionary.Values.ToArray());
            return instance;
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}