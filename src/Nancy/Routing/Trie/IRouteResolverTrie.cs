namespace Nancy.Routing.Trie
{
    public interface IRouteResolverTrie
    {
        void BuildTrie(IRouteCache cache);

        MatchResult[] GetMatches(string method, string path);
    }
}