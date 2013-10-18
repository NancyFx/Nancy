namespace Nancy.Routing.Trie.Nodes
{
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Routing.Constraints;

    /// <summary>
    /// A node for constraint captures e.g. {foo:alpha}, {foo:datetime}
    /// </summary>
    public class CaptureNodeWithConstraint : TrieNode
    {
        private readonly IEnumerable<IRouteConstraint> routeConstraints;
        private string parameterName;
        private string constraint;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 1000; }
        }

        public CaptureNodeWithConstraint(TrieNode parent, string segment, ITrieNodeFactory nodeFactory, IEnumerable<IRouteConstraint> routeConstraints)
            : base(parent, segment, nodeFactory)
        {
            this.routeConstraints = routeConstraints;
            this.ExtractParameterName();
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var routeConstraint = routeConstraints.FirstOrDefault(x => x.Matches(constraint));
            if (routeConstraint == null)
            {
                return SegmentMatch.NoMatch;
            }

            return routeConstraint.GetMatch(this.constraint, segment, this.parameterName);
        }

        private void ExtractParameterName()
        {
            var segmentSplit = this.RouteDefinitionSegment.Trim('{', '}').Split(':');

            this.parameterName = segmentSplit[0];
            this.constraint = segmentSplit[1].ToLowerInvariant();
        }
    }
}
