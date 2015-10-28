namespace Nancy.Routing.Trie.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A base class representing a node in the route trie
    /// </summary>
    public abstract class TrieNode
    {
        private readonly ITrieNodeFactory nodeFactory;

        /// <summary>
        /// Gets or sets the parent node
        /// </summary>
        public TrieNode Parent { get; protected set; }

        /// <summary>
        /// Gets or sets the segment from the route definition that this node represents
        /// </summary>
        public string RouteDefinitionSegment { get; protected set; }

        /// <summary>
        /// Gets or sets the children of this node
        /// </summary>
        public IDictionary<string, TrieNode> Children { get; protected set; }

        /// <summary>
        /// Gets or sets the node data stored at this node, which will be converted
        /// into the <see cref="MatchResult"/> if a match is found
        /// </summary>
        public IList<NodeData> NodeData { get; protected set; }

        /// <summary>
        /// Additional parameters to set that can be determined at trie build time
        /// </summary>
        public IDictionary<string, object> AdditionalParameters { get; protected set; }

        /// <summary>
        /// Score for this node
        /// </summary>
        public abstract int Score { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrieNode"/> class
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment of the route definition</param>
        /// <param name="nodeFactory">Factory for creating new nodes</param>
        protected TrieNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
        {
            this.nodeFactory = nodeFactory;
            this.Parent = parent;
            this.RouteDefinitionSegment = segment;

            this.Children = new Dictionary<string, TrieNode>(StaticConfiguration.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            this.AdditionalParameters = new Dictionary<string, object>();
            this.NodeData = new List<NodeData>();
        }

        /// <summary>
        /// Add a new route to the trie
        /// </summary>
        /// <param name="segments">The segments of the route definition</param>
        /// <param name="moduleType">The module key the route comes from</param>
        /// <param name="routeIndex">The route index in the module</param>
        /// <param name="routeDescription">The route description</param>
        public void Add(string[] segments, Type moduleType, int routeIndex, RouteDescription routeDescription)
        {
            this.Add(segments, -1, 0, 0, moduleType, routeIndex, routeDescription);
        }

        /// <summary>
        /// Add a new route to the trie
        /// </summary>
        /// <param name="segments">The segments of the route definition</param>
        /// <param name="currentIndex">Current index in the segments array</param>
        /// <param name="currentScore">Current score for this route</param>
        /// <param name="nodeCount">Number of nodes added for this route</param>
        /// <param name="moduleType">The module key the route comes from</param>
        /// <param name="routeIndex">The route index in the module</param>
        /// <param name="routeDescription">The route description</param>
        public virtual void Add(string[] segments, int currentIndex, int currentScore, int nodeCount, Type moduleType, int routeIndex, RouteDescription routeDescription)
        {
            if (this.NoMoreSegments(segments, currentIndex))
            {
                this.NodeData.Add(this.BuildNodeData(nodeCount, currentScore + this.Score, moduleType, routeIndex, routeDescription));
                return;
            }

            nodeCount++;
            currentIndex++;
            TrieNode child;

            if (!this.Children.TryGetValue(segments[currentIndex], out child))
            {
                child = this.nodeFactory.GetNodeForSegment(this, segments[currentIndex]);
                this.Children.Add(segments[currentIndex], child);
            }

            child.Add(segments, currentIndex, currentScore + this.Score, nodeCount, moduleType, routeIndex, routeDescription);
        }

        /// <summary>
        /// Gets all matches for a given requested route
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public virtual IEnumerable<MatchResult> GetMatches(string[] segments, NancyContext context)
        {
            return this.GetMatches(segments, 0, new Dictionary<string, object>(this.AdditionalParameters), context);
        }

        /// <summary>
        /// Gets all matches for a given requested route
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index in the route segments</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public virtual IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters, NancyContext context)
        {
            var segmentMatch = this.Match(segments[currentIndex]);
            if (segmentMatch == SegmentMatch.NoMatch)
            {
                return MatchResult.NoMatches;
            }

            if (this.NoMoreSegments(segments, currentIndex))
            {
                return this.BuildResults(capturedParameters, segmentMatch.CapturedParameters) ?? MatchResult.NoMatches;
            }

            currentIndex++;

            return this.GetMatchingChildren(segments, currentIndex, capturedParameters, segmentMatch.CapturedParameters, context);
        }

        /// <summary>
        /// Gets a string representation of all routes
        /// </summary>
        /// <returns>Collection of strings, each representing a route</returns>
        public virtual IEnumerable<string> GetRoutes()
        {
            var routeList = new List<string>(this.Children.Values.SelectMany(c => c.GetRoutes())
                             .Select(s => (this.RouteDefinitionSegment ?? string.Empty) + "/" + s));

            if (this.NodeData.Any())
            {
                var node = this.NodeData.First();
                var resultData = string.Format("{0} (Segments: {1} Score: {2})", this.RouteDefinitionSegment ?? "/", node.RouteLength, node.Score);
                routeList.Add(resultData);
            }

            return routeList;
        }

        /// <summary>
        /// Build the node data that will be used to create the <see cref="MatchResult"/>
        /// We calculate/store as much as possible at build time to reduce match time.
        /// </summary>
        /// <param name="nodeCount">Number of nodes in the route</param>
        /// <param name="score">Score for the route</param>
        /// <param name="moduleType">The module key the route comes from</param>
        /// <param name="routeIndex">The route index in the module</param>
        /// <param name="routeDescription">The route description</param>
        /// <returns>A NodeData instance</returns>
        protected virtual NodeData BuildNodeData(int nodeCount, int score, Type moduleType, int routeIndex, RouteDescription routeDescription)
        {
            return new NodeData
                       {
                           Method = routeDescription.Method,
                           RouteIndex = routeIndex,
                           RouteLength = nodeCount,
                           Score = score,
                           Condition = routeDescription.Condition,
                           ModuleType = moduleType,
                       };
        }

        /// <summary>
        /// Returns whether we are at the end of the segments
        /// </summary>
        /// <param name="segments">Route segments</param>
        /// <param name="currentIndex">Current index</param>
        /// <returns>True if no more segments left, false otherwise</returns>
        protected bool NoMoreSegments(string[] segments, int currentIndex)
        {
            return currentIndex >= segments.Length - 1;
        }

        /// <summary>
        /// Build the results collection from the captured parameters if
        /// this node is the end result
        /// </summary>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="localCaptures">Parameters captured by the local matching</param>
        /// <returns>Array of <see cref="MatchResult"/> objects corresponding to each set of <see cref="NodeData"/> stored at this node</returns>
        protected IEnumerable<MatchResult> BuildResults(IDictionary<string, object> capturedParameters, IDictionary<string, object> localCaptures)
        {
            if (!this.NodeData.Any())
            {
                return MatchResult.NoMatches;
            }

            var parameters = new Dictionary<string, object>(capturedParameters);

            if (this.AdditionalParameters.Any())
            {
                foreach (var additionalParameter in this.AdditionalParameters)
                {
                    if (!parameters.ContainsKey(additionalParameter.Key))
                    {
                        parameters[additionalParameter.Key] = additionalParameter.Value;
                    }
                }
            }

            if (localCaptures.Any())
            {
                foreach (var localCapture in localCaptures)
                {
                    parameters[localCapture.Key] = localCapture.Value;
                }
            }

            return this.NodeData.Select(n => n.ToResult(parameters));
        }

        /// <summary>
        /// Gets all the matches from this node's children
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="localCaptures">Parameters captured by the local matching</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>Collection of <see cref="MatchResult"/> objects</returns>
        protected IEnumerable<MatchResult> GetMatchingChildren(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters, IDictionary<string, object> localCaptures, NancyContext context)
        {
            var parameters = capturedParameters;
            if (localCaptures.Any() || this.AdditionalParameters.Any())
            {
                parameters = new Dictionary<string, object>(parameters);

                foreach (var localParameter in localCaptures)
                {
                    parameters[localParameter.Key] = localParameter.Value;
                }

                foreach (var additionalParameter in this.AdditionalParameters)
                {
                    parameters[additionalParameter.Key] = additionalParameter.Value;
                }
            }

            foreach (var childNode in this.Children.Values)
            {
                foreach (var match in childNode.GetMatches(segments, currentIndex, parameters, context))
                {
                    yield return match;
                }
            }
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public abstract SegmentMatch Match(string segment);
    }
}