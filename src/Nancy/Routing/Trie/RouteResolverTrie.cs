namespace Nancy.Routing.Trie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nancy.Routing.Trie.Nodes;

    /// <summary>
    /// The default route resolution trie
    /// </summary>
    public class RouteResolverTrie : IRouteResolverTrie
    {
        private readonly ITrieNodeFactory nodeFactory;

        private readonly IDictionary<string, TrieNode> routeTries = new Dictionary<string, TrieNode>();

        private static char[] splitSeparators = new[] {'/'};

        public RouteResolverTrie(ITrieNodeFactory nodeFactory)
        {
            this.nodeFactory = nodeFactory;
        }

        /// <summary>
        /// Build the trie from the route cache
        /// </summary>
        /// <param name="cache">The route cache</param>
        public void BuildTrie(IRouteCache cache)
        {
            foreach (var cacheItem in cache)
            {
                var moduleKey = cacheItem.Key;
                var routeDefinitions = cacheItem.Value;

                foreach (var routeDefinition in routeDefinitions)
                {
                    var routeIndex = routeDefinition.Item1;
                    var routeDescription = routeDefinition.Item2;

                    TrieNode trieNode;
                    if (!this.routeTries.TryGetValue(routeDescription.Method, out trieNode))
                    {
                        trieNode = this.nodeFactory.GetNodeForSegment(null, null);

                        this.routeTries.Add(routeDefinition.Item2.Method, trieNode);
                    }

                    var segments = routeDefinition.Item2.Segments.ToArray();

                    trieNode.Add(segments, moduleKey, routeIndex, routeDescription);
                }
            }
        }

        /// <summary>
        /// Get all matches for the given method and path
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">Requested path</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>An array of <see cref="MatchResult"/> elements</returns>
        public MatchResult[] GetMatches(string method, string path, NancyContext context)
        {
            if (string.IsNullOrEmpty(path))
            {
                return MatchResult.NoMatches;
            }

            // TODO -concurrent if allowing updates?
            if (!this.routeTries.ContainsKey(method))
            {
                return MatchResult.NoMatches;
            }

            return this.routeTries[method].GetMatches(path.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries), context)
                                          .ToArray();
        }

        /// <summary>
        /// Get all method options for the given path
        /// </summary>
        /// <param name="path">Requested path</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of strings, each representing an allowed method</returns>
        public IEnumerable<string> GetOptions(string path, NancyContext context)
        {
            foreach (var method in this.routeTries.Keys)
            {
                if (this.GetMatches(method, path, context).Any())
                {
                    yield return method;
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var kvp in this.routeTries)
            {
                var method = kvp.Key;
                sb.Append(
                    kvp.Value.GetRoutes().Select(s => method + " " + s)
                             .Aggregate((r1, r2) => r1 + "\n" + r2));
            }

            return sb.ToString();
        }
    }
}