namespace Nancy.Routing.Trie.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A greedy capture node e.g. {greedy*}
    /// e.g. /foo/bar/{greedy*} - this node will be hit for /foo/bar/[anything that doesn't match another route], but
    /// not for just /foo/bar
    /// e.g. /foo/{greedy*}/bar - this node will be hit for /foo/[anything that doesn't match another route]/bar
    /// </summary>
    public class GreedyCaptureNode : TrieNode
    {
        private string parameterName;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 0; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreedyCaptureNode"/> class.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment of the route definition</param>
        /// <param name="nodeFactory">Factory for creating new nodes</param>
        public GreedyCaptureNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.GetParameterName();
        }

        /// <summary>
        /// Gets all matches for a given requested route
        /// Overridden to handle greedy capturing
        /// </summary>
        /// <param name="segments">Requested route segments</param>
        /// <param name="currentIndex">Current index in the route segments</param>
        /// <param name="capturedParameters">Currently captured parameters</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of <see cref="MatchResult"/> objects</returns>
        public override IEnumerable<MatchResult> GetMatches(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters, NancyContext context)
        {
            var fullGreedy = this.GetFullGreedy(segments, currentIndex, capturedParameters);
            if (!this.Children.Any())
            {
                return fullGreedy;
            }

            var sb = new StringBuilder(segments[currentIndex]);
            var results = new List<MatchResult>();
            currentIndex++;

            while (!this.NoMoreSegments(segments, currentIndex - 1))
            {
                var currentSegment = segments[currentIndex];

                TrieNode matchingChild;
                if (this.Children.TryGetValue(currentSegment, out matchingChild))
                {
                    var parameters = new Dictionary<string, object>(capturedParameters);
                    parameters[this.parameterName] = sb.ToString();
                    results.AddRange(matchingChild.GetMatches(segments, currentIndex, parameters, context));
                }

                sb.AppendFormat("/{0}", currentSegment);
                currentIndex++;
            }

            if (!results.Any())
            {
                results.AddRange(fullGreedy);
            }

            return results;
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// Not-required or called for this node type
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            throw new NotSupportedException();
        }

        private IEnumerable<MatchResult> GetFullGreedy(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters)
        {
            if (!this.NodeData.Any())
            {
                return ArrayCache.Empty<MatchResult>();
            }

            var value = segments.Skip(currentIndex).Aggregate((seg1, seg2) => seg1 + "/" + seg2);
            capturedParameters[this.parameterName] = value;

            return this.NodeData.Select(nd => nd.ToResult(capturedParameters));
        }

        private void GetParameterName()
        {
            this.parameterName = this.RouteDefinitionSegment.Trim('{', '}').TrimEnd('*');
        }
    }
}