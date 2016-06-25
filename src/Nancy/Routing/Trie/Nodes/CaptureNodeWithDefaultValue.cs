namespace Nancy.Routing.Trie.Nodes
{
    using System;

    /// <summary>
    /// A capture node with a default value e.g. {foo?default}
    /// </summary>
    public class CaptureNodeWithDefaultValue : CaptureNode
    {
        private string parameterName;

        private string defaultValue;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 1000; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureNodeWithDefaultValue"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="nodeFactory">The node factory.</param>
        public CaptureNodeWithDefaultValue(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterNameAndDefaultValue();
        }

        /// <summary>
        /// Add a new route to the trie
        /// Adds itself as a normal capture node, but also sets a default capture
        /// on the parent and adds this node's children as children of the parent
        /// too (so it can effectively be "skipped" during matching)
        /// </summary>
        /// <param name="segments">The segments of the route definition</param>
        /// <param name="currentIndex">Current index in the segments array</param>
        /// <param name="currentScore">Current score for this route</param>
        /// <param name="nodeCount">Number of nodes added for this route</param>
        /// <param name="moduleType">The module key the route comes from</param>
        /// <param name="routeIndex">The route index in the module</param>
        /// <param name="routeDescription">The route description</param>
        public override void Add(string[] segments, int currentIndex, int currentScore, int nodeCount, Type moduleType, int routeIndex, RouteDescription routeDescription)
        {
            base.Add(segments, currentIndex, currentScore, nodeCount, moduleType, routeIndex, routeDescription);

            this.Parent.AdditionalParameters[this.parameterName] = this.defaultValue;

            // Keep the same index, reduce the node count and the score
            this.Parent.Add(segments, currentIndex, currentScore - this.Parent.Score, nodeCount - 1, moduleType, routeIndex, routeDescription);
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters[this.parameterName] = segment;
            return match;
        }

        private void ExtractParameterNameAndDefaultValue()
        {
            var elements = this.RouteDefinitionSegment.Trim('{', '}').Split('?');

            if (elements.Length != 2)
            {
                throw new InvalidOperationException(string.Format("Invalid capture route: {0}", this.RouteDefinitionSegment));
            }

            this.parameterName = elements[0];
            this.defaultValue = elements[1];
        }
    }
}