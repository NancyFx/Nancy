namespace Nancy.Routing.Trie
{
    using System.Collections.Generic;

    /// <summary>
    /// Helpers methods for NodeData
    /// </summary>
    public static class NodeDataExtensions
    {
        /// <summary>
        /// Converts a <see cref="NodeData"/> instance into a <see cref="MatchResult"/>
        /// </summary>
        /// <param name="data">Node data</param>
        /// <param name="parameters">Captured parameters</param>
        /// <returns>A <see cref="MatchResult"/> instance</returns>
        public static MatchResult ToResult(this NodeData data, IDictionary<string, object> parameters)
        {
            return new MatchResult(parameters)
            {
                ModuleType = data.ModuleType,
                Method = data.Method,
                RouteIndex = data.RouteIndex,
                RouteLength = data.RouteLength,
                Condition = data.Condition,
                Score = data.Score
            };
        }
    }
}