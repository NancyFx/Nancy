namespace Nancy.Routing.Trie.Nodes
{
    public class OptionalCaptureNode : TrieNode
    {
        private string parameterName;

        public override int Score
        {
            get { return 1000; }
        }

        public OptionalCaptureNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterName();
        }

        private void ExtractParameterName()
        {
            this.parameterName = this.RouteDefinitionSegment.Trim('{', '}').TrimEnd('?');
        }

        public override void Add(string[] segments, int currentIndex, int currentScore, int nodeCount, string moduleKey, int routeIndex, RouteDescription routeDescription)
        {
            base.Add(segments, currentIndex, currentScore, nodeCount, moduleKey, routeIndex, routeDescription);

            // Keep the same index, reduce the node count and the score
            this.Parent.Add(segments, currentIndex, currentScore - this.Parent.Score, nodeCount - 1, moduleKey, routeIndex, routeDescription);
        }

        public override SegmentMatch Match(string segment)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters[this.parameterName] = segment;
            return match;
        }
    }
}