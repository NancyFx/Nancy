namespace Nancy.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Extensions;

    /// <summary>
    /// Default implementation of a route pattern matcher.
    /// </summary>
    public class DefaultRoutePatternMatcher : IRoutePatternMatcher
    {
        private readonly ConcurrentDictionary<string, Regex> matcherCache = new ConcurrentDictionary<string, Regex>();

        public IRoutePatternMatchResult Match(string requestedPath, string routePath)
        {
            var routePathPattern = 
                this.matcherCache.GetOrAdd(routePath, s => BuildRegexMatcher(routePath));

            requestedPath = 
                TrimTrailingSlashFromRequestedPath(requestedPath);

            var match = 
                routePathPattern.Match(requestedPath);

            return new RoutePatternMatchResult(
                match.Success,
                GetParameters(routePathPattern, match.Groups));
        }

        private static string TrimTrailingSlashFromRequestedPath(string requestedPath)
        {
            if (!requestedPath.Equals("/"))
            {
                requestedPath = requestedPath.TrimEnd('/');
            }

            return requestedPath;
        }

        private static Regex BuildRegexMatcher(string path)
        {
            var segments =
                path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            var parameterizedSegments =
                GetParameterizedSegments(segments);

            var pattern =
                string.Concat(@"^/", string.Join("/", parameterizedSegments), @"$");

            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        private static DynamicDictionary GetParameters(Regex regex, GroupCollection groups)
        {
            dynamic data = new DynamicDictionary();

            for (var i = 1; i <= groups.Count; i++)
            {
                if (groups[i].Success)
                {
                    data[regex.GroupNameFromNumber(i)] = groups[i].Value;
                }
            }

            return data;
        }

        private static IEnumerable<string> GetParameterizedSegments(IEnumerable<string> segments)
        {
            foreach (var segment in segments)
            {
                var current = segment;
                if (current.IsParameterized())
                {
                    var replacement =
                        string.Format(CultureInfo.InvariantCulture, @"(?<{0}>(.+?))", segment.GetParameterName());

                    current = segment.Replace(segment, replacement);
                }

                yield return current;
            }
        }
    }
}