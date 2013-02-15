namespace Nancy.Routing.Trie
{
    using System;
    using System.Collections.Generic;

    using Nancy;

    public class NodeData
    {
        /// <summary>
        /// Module key from the matching module
        /// </summary>
        public string ModuleKey { get; set; }

        /// <summary>
        /// Route method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Index in the routing table
        /// </summary>
        public int RouteIndex { get; set; }

        /// <summary>
        /// Number of route segments
        /// </summary>
        public int RouteLength { get; set; }

        /// <summary>
        /// Route score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Route condition
        /// </summary>
        public Func<NancyContext, bool> Condition { get; set; }
    }
}