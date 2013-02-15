namespace Nancy.Routing.Trie.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class TrieNode
    {
        private readonly TrieNodeFactory nodeFactory;

        public TrieNode Parent { get; protected set; }

        public string RouteDefinitionSegment { get; protected set; }

        public IDictionary<string, TrieNode> Children { get; protected set; }

        public IList<NodeData> NodeData { get; protected set; }

        /// <summary>
        /// Additional parameters that can be determined at trie build time
        /// </summary>
        public IDictionary<string, object> AdditionalParameters { get; protected set; }

        public abstract int Score { get; }

        protected TrieNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
        {
            this.nodeFactory = nodeFactory;
            this.Parent = parent;
            this.RouteDefinitionSegment = segment;

            this.Children = new Dictionary<string, TrieNode>();
            this.AdditionalParameters = new Dictionary<string, object>();
            this.NodeData = new List<NodeData>();
        }

        public void Add(string[] segments, string moduleKey, int routeIndex, RouteDescription routeDescription)
        {
            this.Add(segments, -1, 0, 0, moduleKey, routeIndex, routeDescription);    
        }

        public virtual void Add(string[] segments, int currentIndex, int currentScore, int nodeCount, string moduleKey, int routeIndex, RouteDescription routeDescription)
        {
            if (this.NoMoreSegments(segments, currentIndex))
            {
                this.NodeData.Add(this.BuildNodeData(nodeCount, currentScore + this.Score, moduleKey, routeIndex, routeDescription));
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

            child.Add(segments, currentIndex, currentScore + this.Score, nodeCount, moduleKey, routeIndex, routeDescription);
        }

        protected virtual NodeData BuildNodeData(int nodeCount, int score, string moduleKey, int routeIndex, RouteDescription routeDescription)
        {
            // TODO - build this - we need some more properties sending in
            return new NodeData
                       {
                           Method = routeDescription.Method,
                           RouteIndex = routeIndex,
                           RouteLength = nodeCount,
                           Score = score,
                           Condition = routeDescription.Condition,
                           ModuleKey = moduleKey,
                       };
        }

        protected bool NoMoreSegments(string[] segments, int currentIndex)
        {
            return currentIndex >= segments.Length - 1;
        }

        public virtual IEnumerable<MatchResult> GetMatches(string[] segments)
        {
            return this.GetMatches(segments, 0, new Dictionary<string, object>());
        }

        public virtual IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters)
        {
            this.AddAdditionalParameters(capturedParameters);

            var segmentMatch = this.Match(segments[currentIndex]);
            if (segmentMatch == SegmentMatch.NoMatch)
            {
                return MatchResult.NoMatches;
            }

            foreach (var capturedParameter in segmentMatch.CapturedParameters)
            {
                capturedParameters[capturedParameter.Key] = capturedParameter.Value;
            }

            if (this.NoMoreSegments(segments, currentIndex))
            {
                return this.BuildResults(capturedParameters) ?? MatchResult.NoMatches;
            }

            currentIndex++;

            return this.GetMatchingChildren(segments, currentIndex, capturedParameters);
        }

        protected void AddAdditionalParameters(IDictionary<string, object> capturedParameters)
        {
            foreach (var additionalParameter in this.AdditionalParameters)
            {
                capturedParameters[additionalParameter.Key] = additionalParameter.Value;
            }
        }

        protected IEnumerable<MatchResult> BuildResults(IDictionary<string, object> capturedParameters)
        {
            if (!this.NodeData.Any())
            {
                return null;
            }

            return this.NodeData.Select(n => n.ToResult(capturedParameters));
        }

        protected IEnumerable<MatchResult> GetMatchingChildren(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters)
        {
            return this.Children.Values.SelectMany(k => k.GetMatches(segments, currentIndex, new Dictionary<string, object>(capturedParameters)));
        }

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

        public abstract SegmentMatch Match(string segment);
    }
}