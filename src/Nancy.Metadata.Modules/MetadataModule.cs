namespace Nancy.Metadata.Modules
{
    using System;
    using System.Collections.Generic;

    using Nancy.Routing;

    /// <summary>
    /// Base class containing the functionality for obtaining metadata for a given <see cref="RouteDescription"/>.
    /// </summary>
    public abstract class MetadataModule<TMetadata> : IMetadataModule where TMetadata : class
    {
        private readonly IDictionary<string, Func<RouteDescription, TMetadata>> metadata;

        protected MetadataModule()
        {
            this.metadata = new Dictionary<string, Func<RouteDescription, TMetadata>>();
        }

        /// <summary>
        /// Gets <see cref="RouteMetadataBuilder"/> for describing routes.
        /// </summary>
        /// <value>A <see cref="RouteMetadataBuilder"/> instance.</value>
        public RouteMetadataBuilder Describe
        {
            get { return new RouteMetadataBuilder(this); }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of metadata based on <typeparamref name="TMetadata" />.
        /// </summary>
        public Type MetadataType
        {
            get
            {
                return typeof(TMetadata);
            }
        }

        /// <summary>
        /// Returns metadata for the given <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description">The route to obtain metadata for.</param>
        /// <returns>An instance of <see cref="MetadataType"/> if one exists, otherwise null.</returns>
        public object GetMetadata(RouteDescription description)
        {
            if (this.metadata.ContainsKey(description.Name))
            {
                return this.metadata[description.Name].Invoke(description);
            }

            return null;
        }

        /// <summary>
        /// Helper class for configuring a route metadata handler in a module.
        /// </summary>
        public class RouteMetadataBuilder
        {
            private readonly MetadataModule<TMetadata> parentModule;

            /// <summary>
            /// Initializes a new instance of the <see cref="RouteMetadataBuilder"/> class.
            /// </summary>
            /// <param name="metadataModule">The <see cref="MetadataModule{TMetadata}"/> that the route is being configured for.</param>
            public RouteMetadataBuilder(MetadataModule<TMetadata> metadataModule)
            {
                this.parentModule = metadataModule;
            }

            /// <summary>
            /// Describes metadata for a route with the specified <paramref name="name"/>.
            /// </summary>
            /// <value>A delegate that is used to return the route metadata.</value>
            public Func<RouteDescription, TMetadata> this[string name]
            {
                set { this.AddRouteMetadata(name, value); }
            }

            protected void AddRouteMetadata(string name, Func<RouteDescription, TMetadata> value)
            {
                this.parentModule.metadata.Add(name, value);
            }
        }
    }
}
