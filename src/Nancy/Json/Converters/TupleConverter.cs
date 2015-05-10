namespace Nancy.Json.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class TupleConverter : JavaScriptConverter
    {
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

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var ctor = type.GetConstructors().First();
            object instance = ctor.Invoke(dictionary.Values.ToArray());
            return instance;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}