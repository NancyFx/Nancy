namespace Nancy.Owin
{
    using System.Collections.Generic;
    using System.Linq;

    internal static class DictionaryExtensions
    {
        internal static T Get<T>(this IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : default(T);
        }

        internal static string GetHeader(this IDictionary<string, string[]> headers, string key)
        {
            string[] value;
            return headers.TryGetValue(key, out value) && value != null ? string.Join(",", value.ToArray()) : null;
        }
    }
}