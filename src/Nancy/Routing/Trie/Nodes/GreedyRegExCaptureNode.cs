namespace Nancy.Routing.Trie.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A greedy regular expression capture node e.g. ^(?&lt;id>\d{0,100})$
    /// For use on an entire route path, regular expression must be surrounded by ^( )$
    /// e.g. @"^(?:(?&lt;id>videos/\d{1,10})(?:/{0,1}(?&lt;slug>.*)))$"
    /// This will match for a Url like /videos/123/some-random-slug
    /// and capture 'videos/123' and 'some-random-slug'
    /// </summary>
    public class GreedyRegExCaptureNode : TrieNode
    {
        private Regex expression;
        private string[] groupNames;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 100; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreedyRegExCaptureNode"/> class.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment of the route definition</param>
        /// <param name="nodeFactory">Factory for creating new nodes</param>
        public GreedyRegExCaptureNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.BuildRegEx();
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
            var value = segments.Skip(currentIndex).Aggregate((seg1, seg2) => seg1 + "/" + seg2);
            var match = this.expression.Match(value);

            if (!match.Success)
            {
                return ArrayCache.Empty<MatchResult>();
            }

            foreach (var groupName in this.groupNames)
            {
                var group = match.Groups[groupName];

                if (group.Success)
                {
                    capturedParameters.Add(groupName, group.Value);
                }
            }

            return this.NodeData.Select(nd => nd.ToResult(capturedParameters));
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

        private void BuildRegEx()
        {
            this.expression = new Regex(this.RouteDefinitionSegment, RegexOptions.Compiled);
            this.groupNames = this.expression.GetGroupNames();
        }
    }
}