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
        private readonly ConcurrentDictionary<string, Tuple<Regex, IEnumerable<ParameterSegmentInformation>>>
            matcherCache = new ConcurrentDictionary<string, Tuple<Regex, IEnumerable<ParameterSegmentInformation>>>();

        /// <summary>
        /// Attempts to match a requested path with a route pattern.
        /// </summary>
        /// <param name="requestedPath">The path that was requested.</param>
        /// <param name="routePath">The route pattern that the requested path should be attempted to be matched with.</param>
        /// <param name="segments"></param>
        /// <param name="context">The <see cref="NancyContext"/> instance for the current request.</param>
        /// <returns>An <see cref="IRoutePatternMatchResult"/> instance, containing the outcome of the match.</returns>
        public IRoutePatternMatchResult Match(string requestedPath, string routePath, IEnumerable<string> segments, NancyContext context)
        {
            var routePathPattern =
                this.matcherCache.GetOrAdd(routePath, s => BuildRegexMatcher(segments.ToList()));

            requestedPath =
                TrimTrailingSlashFromRequestedPath(requestedPath);

            var match =
                routePathPattern.Item1.Match(requestedPath);

            var matches = match
                .Groups.Cast<Group>()
                .ToList();

            return new RoutePatternMatchResult(
                match.Success,
                GetParameters(routePathPattern, matches),
                context);
        }

        private static string TrimTrailingSlashFromRequestedPath(string requestedPath)
        {
            if (!requestedPath.Equals("/"))
            {
                requestedPath = requestedPath.TrimEnd('/');
            }

            return requestedPath;
        }

        private static Tuple<Regex, IEnumerable<ParameterSegmentInformation>> BuildRegexMatcher(IList<string> segments)
        {
            var parameterizedSegments =
                GetParameterizedSegments2(segments);

            var parsedSegments = (segments.Any()) ?
                string.Join(string.Empty, parameterizedSegments.Item1) :
                "/";

            var pattern =
                string.Concat(@"^", parsedSegments, @"$");

            return new Tuple<Regex, IEnumerable<ParameterSegmentInformation>>(
                new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase),
                parameterizedSegments.Item2);
        }

        private static DynamicDictionary GetParameters(Tuple<Regex, IEnumerable<ParameterSegmentInformation>> result, IList<Group> matches)
        {
            dynamic data = new DynamicDictionary();

            for (var i = 1; i <= matches.Count() - 1; i++)
            {
                var name =
                    result.Item1.GroupNameFromNumber(i);

                var value = (matches[i].Success) ?
                    matches[i].Value :
                    result.Item2.Where(x => x.Name.Equals(name) && !string.IsNullOrEmpty(x.DefaultValue)).Select(x => x.DefaultValue).SingleOrDefault();

                data[name] = value;
            }

            return data;
        }

        private static Tuple<IEnumerable<string>, IEnumerable<ParameterSegmentInformation>> GetParameterizedSegments2(
            IEnumerable<string> segments)
        {
            var parsedSegments = new List<string>();
            var segmentInformation = new List<ParameterSegmentInformation>();

            foreach (var segment in segments)
            {
                var current = segment;

                if (current.IsParameterized() && !IsRegexSegment(current))
                {
                    var result =
                        ParameterizeSegment(segment);

                    current = result.Item1;
                    segmentInformation.AddRange(result.Item2);
                }
                else
                {
                    current = string.Concat("/", (!IsRegexSegment(current)) ? Regex.Escape(current) : current);
                }

                parsedSegments.Add(current);
            }

            return new Tuple<IEnumerable<string>, IEnumerable<ParameterSegmentInformation>>(parsedSegments, segmentInformation);
        }

        private static bool IsRegexSegment(string segment)
        {
            return segment.StartsWith("(");
        }

        private static Tuple<string, IEnumerable<ParameterSegmentInformation>> ParameterizeSegment(string segment)
        {
            segment = segment.Replace(".", @"\.");

            var details =
                segment.GetParameterDetails().ToList();

            for (var index = 0; index < details.Count; index++)
            {
                var information =
                    details.Skip(index).First();

                var replacement =
                    string.Format(CultureInfo.InvariantCulture, @"(?<{0}>.+?)", information.Name);

                if (information.IsOptional)
                {
                    replacement = string.Concat(replacement, "?");
                }

                segment = segment.Replace(
                    string.Concat("{", information.FullSegmentName, "}"),
                    replacement);
            }

            segment = string.Concat(@"\/", segment);

            if (details.All(x => x.IsOptional) && segment.StartsWith(@"\/(") && (segment.EndsWith(")") || segment.EndsWith(")?")))
            {
                segment = string.Concat(@"(?:", segment, ")?");
            }

            return new Tuple<string, IEnumerable<ParameterSegmentInformation>>(segment, details);
        }
    }
}
