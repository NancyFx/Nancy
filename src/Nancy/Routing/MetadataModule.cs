namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Basic class containing the functionality for providing route metadata for <see cref="INancyModule"/> classes. 
    /// </summary>
    public abstract class MetadataModule<T> : IMetadataModule where T : class
    {
        private readonly IDictionary<string, Func<RouteDescription, T>> metadata;

        protected MetadataModule()
        {
            this.metadata = new Dictionary<string, Func<RouteDescription, T>>();
        }

        public RouteMetadataBuilder Describe
        {
            get { return new RouteMetadataBuilder(this); }
        }

        public Type MetadataType
        {
            get
            {
                return typeof(T);
            }
        }

        public object ApplyMetadata(RouteDescription description)
        {
            if (this.metadata.ContainsKey(description.Name))
            {
                return this.metadata[description.Name].Invoke(description);
            }

            return null;
        }

        public class RouteMetadataBuilder
        {
            private readonly MetadataModule<T> parentModule;

            public RouteMetadataBuilder(MetadataModule<T> metadataModule)
            {
                this.parentModule = metadataModule;
            }

            public Func<RouteDescription, T> this[string name]
            {
                set { this.AddRouteMetadata(name, value); }
            }

            protected void AddRouteMetadata(string name, Func<RouteDescription, T> value)
            {
                this.parentModule.metadata.Add(name, value);
            }
        }
    }
}