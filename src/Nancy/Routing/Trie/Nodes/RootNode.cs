namespace Nancy.Routing.Trie.Nodes
{
    using System.Collections.Generic;

    /// <summary>
    /// Root node of a trie
    /// </summary>
    public class RootNode : TrieNode
    {
        private SegmentMatch segmentMatch = new SegmentMatch(true);

        public override int Score
        {
            get { return 0; }
        }

        public RootNode(TrieNodeFactory nodeFactory)
            : base(null, null, nodeFactory)
        {
        }

        public override IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters, NancyContext context)
        {
            this.AddAdditionalParameters(capturedParameters);

            if (segments.Length == 0)
            {
                return this.BuildResults(capturedParameters);
            }

            return this.GetMatchingChildren(segments, currentIndex, capturedParameters, context);
        }

        public override SegmentMatch Match(string segment)
        {
            return this.segmentMatch;
        }
    }
}