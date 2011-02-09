namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static IDictionary<string, IEnumerable<string>> ToDictionary(this NameValueCollection source)
        {
            return source.AllKeys.ToDictionary<string, string, IEnumerable<string>>(key => key, source.GetValues);
        }

        public static DynamicDictionary ToDynamicDictionary(this NameValueCollection source)
        {
            var dict = new DynamicDictionary();
            foreach (var key in source.AllKeys)
            {
                var values = source.GetValues(key);
                if (values == null) continue;
                if (values.Length == 1)
                    dict[key] = values[0];
                else
                    dict[key] = values;
            }
            return dict;
        }

        public static NameValueCollection ToNameValueCollection(this IDictionary<string, IEnumerable<string>> source)
        {
            var collection = new NameValueCollection();

            foreach (var key in source.Keys)
            {
                foreach (var value in source[key])
                {
                    collection.Add(key, value);
                }
            }

            return collection;
        }
    }
}