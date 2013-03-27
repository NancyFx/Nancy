namespace Nancy.Routing.Trie
{
    using Nancy.Routing.Trie.Nodes;

    /// <summary>
    /// Factory for creating the correct type of TrieNode
    /// </summary>
    public class TrieNodeFactory : ITrieNodeFactory
    {
        /// <summary>
        /// Gets the correct Trie node type for the given segment
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment</param>
        /// <returns>TrieNode instance</returns>
        public TrieNode GetNodeForSegment(TrieNode parent, string segment)
        {
            if (parent == null)
            {
                return new RootNode(this);
            }

            if (segment.StartsWith("(") && segment.EndsWith(")"))
            {
                return new RegExNode(parent, segment, this);
            }

            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                return this.GetCaptureNode(parent, segment);
            }

            if (segment.StartsWith("^(") && segment.EndsWith(")"))
            {
                return new GreedyRegExCaptureNode(parent, segment, this);
            }

            return new LiteralNode(parent, segment, this);
        }

        private TrieNode GetCaptureNode(TrieNode parent, string segment)
        {
            if (segment.EndsWith("?}"))
            {
                return new OptionalCaptureNode(parent, segment, this);
            }

            if (segment.EndsWith("*}"))
            {
                return new GreedyCaptureNode(parent, segment, this);
            }

            if (segment.Contains("?"))
            {
                return new CaptureNodeWithDefaultValue(parent, segment, this);
            }

            return new CaptureNode(parent, segment, this);
        }
    }
}