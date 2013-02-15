namespace Nancy.Routing.Trie.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A greedy capture node - designed to be a fallback "catch any" segment on the
    /// end of a route.
    /// e.g. /foo/bar/[greedy] - this node will be hit for /foo/bar/[anything that doesn't match another route], but
    /// not for just /foo/bar
    /// </summary>
    public class GreedyCaptureNode : TrieNode
    {
        private string parameterName;

        public override int Score
        {
            get { return 0; }
        }

        public GreedyCaptureNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.GetParameterName();
        }

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

        private IEnumerable<MatchResult> GetFullGreedy(string[] segments, int currentIndex, IDictionary<string, object> capturedParameters)
        {
            if (!this.NodeData.Any())
            {
                return new MatchResult[] { };
            }

            var value = segments.Skip(currentIndex).Aggregate((seg1, seg2) => seg1 + "/" + seg2);
            capturedParameters[this.parameterName] = value;

            return this.NodeData.Select(nd => nd.ToResult(capturedParameters));
        }

        public override SegmentMatch Match(string segment)
        {
            throw new NotSupportedException();
        }

        private void GetParameterName()
        {
            this.parameterName = this.RouteDefinitionSegment.Trim('{', '}').TrimEnd('*');
        }
    }
}