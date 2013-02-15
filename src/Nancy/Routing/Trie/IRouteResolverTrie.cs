namespace Nancy.Routing.Trie
{
    /// <summary>
    /// Trie structure for resolving routes
    /// </summary>
    public interface IRouteResolverTrie
    {
        /// <summary>
        /// Build the trie from the route cache
        /// </summary>
        /// <param name="cache"></param>
        void BuildTrie(IRouteCache cache);

        /// <summary>
        /// Get all matches for the given method and path
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">Reqeusted path</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>An array of <see cref="MatchResult"/> elements</returns>
        MatchResult[] GetMatches(string method, string path, NancyContext context);
    }
}