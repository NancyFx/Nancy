namespace Nancy.Routing.Trie.Nodes
{
    using System;

    using Nancy.Routing;

    public class CaptureNodeWithDefaultValue : CaptureNode
    {
        private string parameterName;

        private string defaultValue;

        public override int Score
        {
            get { return 1000; }
        }

        public CaptureNodeWithDefaultValue(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterNameAndDefaultValue();
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

        public override void Add(string[] segments, int currentIndex, int currentScore, int nodeCount, string moduleKey, int routeIndex, RouteDescription routeDescription)
        {
            base.Add(segments, currentIndex, currentScore, nodeCount, moduleKey, routeIndex, routeDescription);

            this.Parent.AdditionalParameters[this.parameterName] = this.defaultValue;

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