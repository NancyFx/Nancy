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
        private readonly IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints;
        private string parameterName;
        private string constraint;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 2000; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureNodeWithConstraint"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="nodeFactory">The node factory.</param>
        /// <param name="routeSegmentConstraints">The route segment constraints.</param>
        public CaptureNodeWithConstraint(TrieNode parent, string segment, ITrieNodeFactory nodeFactory, IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints)
            : base(parent, segment, nodeFactory)
        {
            this.routeSegmentConstraints = routeSegmentConstraints;
            this.ExtractParameterName();
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var routeSegmentConstraint = routeSegmentConstraints.FirstOrDefault(x => x.Matches(constraint));
            if (routeSegmentConstraint == null)
            {
                return SegmentMatch.NoMatch;
            }

            return routeSegmentConstraint.GetMatch(this.constraint, segment, this.parameterName);
        }

        private void ExtractParameterName()
        {
            var segmentSplit = this.RouteDefinitionSegment.Trim('{', '}').Split(':');

            this.parameterName = segmentSplit[0];
            this.constraint = segmentSplit[1];
        }
    }
}
