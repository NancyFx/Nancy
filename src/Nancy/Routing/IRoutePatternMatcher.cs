namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Extensions;

    public interface IRoutePatternMatchResult
    {
        bool IsMatch { get; }

        DynamicDictionary Parameters { get; }
    }

    public class RoutePatternMatchResult : IRoutePatternMatchResult
    {
        public RoutePatternMatchResult(bool isMatch, DynamicDictionary parameters)
        {
            this.IsMatch = isMatch;
            this.Parameters = parameters;
        }

        public bool IsMatch { get; private set; }
        
        public DynamicDictionary Parameters { get; private set; }
    }

    public interface IRoutePatternMatcher
    {
        IRoutePatternMatchResult Match(string requestedPath, string routePath);
    }

    public class DefaultRoutePatternMatcher : IRoutePatternMatcher
    {
        public IRoutePatternMatchResult Match(string requestedPath, string routePath)
        {
            var routePathPattern =
                BuildRegexMatcher(routePath);

            var match = routePathPattern.Match(requestedPath);

            return new RoutePatternMatchResult(
                match.Success,
                GetParameters(routePath, match.Groups));
        }

        private static DynamicDictionary GetParameters(string path, GroupCollection groups)
        {
            var segments =
                new ReadOnlyCollection<string>(
                    path.Split(new[] { "/" },
                    StringSplitOptions.RemoveEmptyEntries).ToList());

            var parameters =
                from segment in segments
                where segment.IsParameterized()
                select segment.GetParameterName();

            dynamic data =
                new DynamicDictionary();

            foreach (var parameter in parameters)
            {
                data[parameter] = groups[parameter].Value;
            }

            return data;
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

        private static IEnumerable<string> GetParameterizedSegments(IEnumerable<string> segments)
        {
            foreach (var segment in segments)
            {
                var current = segment;
                if (current.IsParameterized())
                {
                    var replacement =
                        string.Format(CultureInfo.InvariantCulture, @"(?<{0}>[/A-Z0-9._-]*)", segment.GetParameterName());

                    current = segment.Replace(segment, replacement);
                }

                yield return current;
            }
        }
    }
}