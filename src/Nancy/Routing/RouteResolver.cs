namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Extensions;

    public class RouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions)
        {
            var matchingRoutes =
                from description in descriptions
                let matcher = BuildRegexMatcher(description)
                let result = matcher.Match(request.Path)
                where result.Success
                select new
                {
                    Groups = result.Groups,
                    Description = description
                };

            var selected = matchingRoutes
                .OrderByDescending(x => GetSegmentCount(x.Description))
                .FirstOrDefault();

            return selected != null ?
                new Route(selected.Description.GetModuleQualifiedPath(), GetParameters(selected.Description, selected.Groups), selected.Description.Action) : 
                new NoMatchingRouteFoundRoute(request.Path);
        }

        private static RouteParameters GetParameters(RouteDescription description, GroupCollection groups)
        {
            var segments =
                new ReadOnlyCollection<string>(
                    description.Path.Split(new[] { "/" },
                    StringSplitOptions.RemoveEmptyEntries).ToList());

            var parameters =
                from segment in segments
                where segment.IsParameterized()
                select segment.GetParameterName();

            dynamic data =
                new RouteParameters();

            foreach (var parameter in parameters)
            {
                data[parameter] = groups[parameter].Value;
            }

            return data;
        }

        private static Regex BuildRegexMatcher(RouteDescription description)
        {
            var segments =
                description.GetModuleQualifiedPath().Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

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
                        string.Format(CultureInfo.InvariantCulture, @"(?<{0}>[/A-Z0-9_-]*)", segment.GetParameterName());

                    current = segment.Replace(segment, replacement);
                }

                yield return current;
            }
        }

        private static int GetSegmentCount(RouteDescription description)
        {
            var moduleQualifiedPath =
                description.GetModuleQualifiedPath();

            var indexOfFirstParameter =
                moduleQualifiedPath.IndexOf('{');

            if (indexOfFirstParameter > -1)
                moduleQualifiedPath = moduleQualifiedPath.Substring(0, indexOfFirstParameter);

            return moduleQualifiedPath.Split('/').Count();
        }
    }
}