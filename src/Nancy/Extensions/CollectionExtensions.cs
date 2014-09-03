namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Containing extensions for the collection objects.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Converts a <see cref="NameValueCollection"/> to a <see cref="IDictionary{TKey,TValue}"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="NameValueCollection"/> to convert.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> instance.</returns>
        public static IDictionary<string, IEnumerable<string>> ToDictionary(this NameValueCollection source)
        {
            return source.AllKeys.ToDictionary<string, string, IEnumerable<string>>(key => key, source.GetValues);
        }

        /// <summary>
        /// Converts an <see cref="IDictionary{TKey,TValue}"/> instance to a <see cref="NameValueCollection"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="IDictionary{TKey,TValue}"/> instance to convert.</param>
        /// <returns>A <see cref="NameValueCollection"/> instance.</returns>
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

        /// <summary>
        /// Merges a collection of <see cref="IDictionary{TKey,TValue}"/> instances into a single one.
        /// </summary>
        /// <param name="dictionaries">The list of <see cref="IDictionary{TKey,TValue}"/> instances to merge.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> instance containg the keys and values from the other instances.</returns>
        public static IDictionary<string, string> Merge(this IEnumerable<IDictionary<string, string>> dictionaries)
        {
            var output =
                new Dictionary<string, string>(StaticConfiguration.CaseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);

            foreach (var dictionary in dictionaries.Where(d => d != null))
            {
                foreach (var kvp in dictionary)
                {
                    if (!output.ContainsKey(kvp.Key))
                    {
                        output.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Filters a collection based on a provided key selector.
        /// </summary>
        /// <param name="source">The collection filter.</param>
        /// <param name="keySelector">The predicate to filter by.</param>
        /// <typeparam name="TSource">The type of the collection to filter.</typeparam>
        /// <typeparam name="TKey">The type of the key to filter by.</typeparam>
        /// <returns>A <see cref="IEnumerable{T}"/> instance with the filtered values.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}