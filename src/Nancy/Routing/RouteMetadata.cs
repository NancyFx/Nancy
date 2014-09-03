namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Stores metadata created by <see cref="IRouteMetadataProvider"/> instances.
    /// </summary>
    public class RouteMetadata
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RouteMetadata"/> class.
        /// </summary>
        /// <param name="metadata">An <see cref="IDictionary{TKey,TValue}"/> containing the metadata, organised by the type that it is stored in.</param>
        public RouteMetadata(IDictionary<Type, object> metadata)
        {
            this.Raw = metadata;
        }

        /// <summary>
        /// Gets the raw metadata <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance.</value>
        public IDictionary<Type, object> Raw { get; private set; }

        /// <summary>
        /// Gets a boolean that indicates if the specific type of metadata is stored.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata to check for.</typeparam>
        /// <returns><see langword="true"/> if metadata, of the requested type is stored, otherwise <see langword="false"/>.</returns>
        public bool Has<TMetadata>()
        {
            return this.Raw.ContainsKey(typeof (TMetadata));
        }

        /// <summary>
        /// Retrieves metadata of the provided type.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata to retrieve.</typeparam>
        /// <returns>The metadata instance if available, otherwise <see langword="null"/>.</returns>
        public TMetadata Retrieve<TMetadata>()
        {
            var key =
                typeof(TMetadata);

            return (this.Raw.ContainsKey(key)) ?
                (TMetadata)this.Raw[key] :
                default(TMetadata);
        }
    }
}