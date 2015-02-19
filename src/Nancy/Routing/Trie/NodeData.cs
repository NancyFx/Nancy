namespace Nancy.Routing.Trie
{
    using System;

    /// <summary>
    /// Represents a route that ends at a particular node.
    /// We store/calculate as much as we can at build time to save
    /// time during route matching.
    /// </summary>
    public class NodeData
    {
        /// <summary>
        /// Gets or sets the module type from the matching module
        /// </summary>
        public Type ModuleType { get; set; }

        /// <summary>
        /// Gets or sets the route method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the index in the module routing table
        /// </summary>
        public int RouteIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of segments in the route
        /// </summary>
        public int RouteLength { get; set; }

        /// <summary>
        /// Gets or sets the route score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the route condition delegate
        /// </summary>
        public Func<NancyContext, bool> Condition { get; set; }
    }
}