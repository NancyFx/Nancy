namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains extensions for the <see cref="IRouteCache"/> type.
    /// </summary>
    public static class RouteCacheExtensions
    {
        /// <summary>
        /// Retrieves metadata for all declared routes.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata to retrieve.</typeparam>
        /// <param name="cache">The <see cref="IRouteCache"/> to retrieve the metadata.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing instances of the <typeparamref name="TMetadata"/> type.</returns>
        public static IEnumerable<TMetadata> RetrieveMetadata<TMetadata>(this IDictionary<Type, List<Tuple<int, RouteDescription>>> cache)
        {
            return cache.Values.SelectMany(x => x.Select(y => y.Item2.Metadata.Retrieve<TMetadata>()));
        }
    }
}