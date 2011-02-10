namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Nancy.Extensions;

    public class DefaultRoutePatternMatcher : IRoutePatternMatcher
    {
        public IRoutePatternMatchResult Match(string requestedPath, string routePath)
        {
            var routePathPattern =
                BuildRegexMatcher(routePath);

            requestedPath =
                TrimTrailingSlashFromRequestedPath(requestedPath);

            var match = routePathPattern.Match(requestedPath);

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

            return new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private static DynamicDictionary GetParameters(Regex regex, GroupCollection groups)
        {
            dynamic data = new DynamicDictionary();

            for (int i = 1; i <= groups.Count; i++)
            {
                data[regex.GroupNameFromNumber(i)] = Uri.UnescapeDataString(groups[i].Value);
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
                        string.Format(CultureInfo.InvariantCulture, @"(?<{0}>[/A-Z0-9%._-]*)", segment.GetParameterName());

                    current = segment.Replace(segment, replacement);
                }

                yield return current;
            }
        }
    }
}