namespace Nancy.Routing.Trie.Nodes
{
    public class CaptureNode : TrieNode
    {
        private string parameterName;

        public override int Score
        {
            get { return 1000; }
        }

        public CaptureNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterName();
        }

        private void ExtractParameterName()
        {
            this.parameterName = this.RouteDefinitionSegment.Trim('{', '}');
        }

        public override SegmentMatch Match(string segment)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters.Add(this.parameterName, segment);
            return match;
        }
    }
}