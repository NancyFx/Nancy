namespace Nancy.Routing.Trie.Nodes
{
    /// <summary>
    /// A node for standard captures e.g. {foo}
    /// </summary>
    public class CaptureNode : TrieNode
    {
        private string parameterName;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 1000; }
        }

        public CaptureNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterName();
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters.Add(this.parameterName, segment);
            return match;
        }

        private void ExtractParameterName()
        {
            this.parameterName = this.RouteDefinitionSegment.Trim('{', '}');
        }
    }
}