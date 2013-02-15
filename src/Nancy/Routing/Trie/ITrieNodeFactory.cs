namespace Nancy.Routing.Trie
{
    using Nancy.Routing.Trie.Nodes;

    public interface ITrieNodeFactory
    {
        /// <summary>
        /// Gets the correct Trie node type for the given segment
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="segment">Segment</param>
        /// <returns>TrieNode instance</returns>
        TrieNode GetNodeForSegment(TrieNode parent, string segment);
    }
}