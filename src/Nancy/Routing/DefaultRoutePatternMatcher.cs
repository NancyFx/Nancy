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

        /// <summary>
        /// Attempts to match a requested path with a route pattern.
        /// </summary>
        /// <param name="requestedPath">The path that was requested.</param>
        /// <param name="routePath">The route pattern that the requested path should be attempted to be matched with.</param>
        /// <param name="context">The <see cref="NancyContext"/> instance for the current request.</param>
        /// <returns>An <see cref="IRoutePatternMatchResult"/> instance, containing the outcome of the match.</returns>
        public IRoutePatternMatchResult Match(string requestedPath, string routePath, NancyContext context)
        {
            var routePathPattern = 
                this.matcherCache.GetOrAdd(routePath, s => BuildRegexMatcher(routePath));

            requestedPath = 
                TrimTrailingSlashFromRequestedPath(requestedPath);

            var matches = routePathPattern
                .Match(requestedPath)
                .Groups.Cast<Group>()
                .Where(x => x.Success)
                .ToList();

            return new RoutePatternMatchResult(
                matches.Any(),
                GetParameters(routePathPattern, matches));
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

        private static DynamicDictionary GetParameters(Regex regex, IList<Group> matches)
        {
            dynamic data = new DynamicDictionary();

            for (var i = 1; i <= matches.Count() - 1; i++)
            {
                data[regex.GroupNameFromNumber(i)] = matches[i].Value;
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
                    current = segment.Replace(".", @"\.");

                    foreach (var name in segment.GetParameterNames())
                    {
                        var replacement =
                            string.Format(CultureInfo.InvariantCulture, @"(?<{0}>.+?)", name);

                        current = current.Replace(
                            string.Concat("{", name, "}"),
                            replacement);
                    }
                }

                yield return current;
            }
        }
    }
}
