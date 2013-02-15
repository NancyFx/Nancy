namespace Nancy.Routing.Trie.Nodes
{
    using System;

    /// <summary>
    /// Literal string node
    /// </summary>
    public class LiteralNode : TrieNode
    {
        public override int Score
        {
            get { return 10000; }
        }

        public LiteralNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
        }

        public override SegmentMatch Match(string segment)
        {
            var comparisonType = Nancy.StaticConfiguration.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            if (string.Equals(
                    segment, 
                    this.RouteDefinitionSegment, 
                    comparisonType))
            {
                return new SegmentMatch(true);
            }

            return SegmentMatch.NoMatch;
        }
    }
}