namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    public class RouteMetadata
    {
        public RouteMetadata(IDictionary<Type, object> metadata)
        {
            this.Raw = metadata;
        }

        public IDictionary<Type, object> Raw { get; private set; }

        public bool Has<TMetadata>()
        {
            return this.Raw.ContainsKey(typeof (TMetadata));
        }

        public TMetadata Retrieve<TMetadata>()
        {
            return (TMetadata)this.Raw[typeof (TMetadata)];
        }
    }
}