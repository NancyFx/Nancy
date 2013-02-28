namespace Nancy.Routing.Trie
{
    using Nancy.Routing.Trie.Nodes;

    /// <summary>
    /// Factory for creating trie nodes from route definition segments
    /// </summary>
    public interface ITrieNodeFactory
    {
        /// <summary>
        /// Gets the correct Trie node type for the given segment
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment</param>
        /// <returns>Corresponding TrieNode instance</returns>
        TrieNode GetNodeForSegment(TrieNode parent, string segment);
    }
}