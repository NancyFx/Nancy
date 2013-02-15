namespace Nancy.Routing.Trie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nancy;
    using Nancy.Routing;

    using Nancy.Routing.Trie.Nodes;

    public class RouteResolverTrie : IRouteResolverTrie
    {
        private readonly ITrieNodeFactory nodeFactory;

        private readonly IDictionary<string, TrieNode> routeTries = new Dictionary<string, TrieNode>();

        private static char[] splitSeparators = new[] {'/'};

        public RouteResolverTrie(ITrieNodeFactory nodeFactory)
        {
            this.nodeFactory = nodeFactory;
        }

        // TODO - need more info in here - module key etc
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

        public MatchResult[] GetMatches(string method, string path)
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

            return this.routeTries[method].GetMatches(path.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries))
                                          .ToArray();
        }

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