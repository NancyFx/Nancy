namespace Nancy.Routing.Trie.Nodes
{
    using System.Collections.Generic;

    /// <summary>
    /// Root node of a trie
    /// </summary>
    public class RootNode : TrieNode
    {
        private SegmentMatch segmentMatch = new SegmentMatch(true);

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 0; }
        }

        public RootNode(TrieNodeFactory nodeFactory)
            : base(null, null, nodeFactory)
        {
        }

        /// <summary>
        /// Gets all matches for a given requested route
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index in the route segments</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public override IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters, NancyContext context)
        {
            this.AddAdditionalParameters(capturedParameters);

            if (segments.Length == 0)
            {
                return this.BuildResults(capturedParameters);
            }

            return this.GetMatchingChildren(segments, currentIndex, capturedParameters, context);
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            return this.segmentMatch;
        }
    }
}