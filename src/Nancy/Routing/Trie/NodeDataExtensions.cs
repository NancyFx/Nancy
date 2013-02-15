namespace Nancy.Routing.Trie
{
    using System.Collections.Generic;

    public static class NodeDataExtensions
    {
        public static MatchResult ToResult(this NodeData data)
        {
            return new MatchResult
            {
                ModuleKey = data.ModuleKey,
                Method = data.Method,
                RouteIndex = data.RouteIndex,
                RouteLength = data.RouteLength,
                Score = data.Score
            };
        }

        public static MatchResult ToResult(this NodeData data, IDictionary<string, object> parameters)
        {
            return new MatchResult(parameters)
            {
                ModuleKey = data.ModuleKey,
                Method = data.Method,
                RouteIndex = data.RouteIndex,
                RouteLength = data.RouteLength,
                Score = data.Score
            };
        }
    }
}