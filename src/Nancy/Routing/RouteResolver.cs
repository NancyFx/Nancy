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
        public IRoute GetRoute(IRequest request, IEnumerable<ModuleMeta> metas, INancyApplication application)
        {            
            var matchingRoutes =
                from meta in metas
                from description in meta.RouteDescriptions
                let matcher = BuildRegexMatcher(description)
                let result = matcher.Match(request.Uri)
                where result.Success
                select new
                {
                    Groups = result.Groups,
                    Meta = meta,
                    Description = description
                };

            var selected = matchingRoutes
                .OrderByDescending(x => GetSegmentCount(x.Description))
                .FirstOrDefault();            

            if (selected == null)
            {
                return new NoMatchingRouteFoundRoute(request.Uri);
            }

            var instance = application.Activator.CreateInstance(selected.Meta.Type);
            instance.Application = application;
            instance.Request = request;
            var action = instance.GetRoutes(selected.Description.Method)[selected.Description.Path];
            return new Route(selected.Description.GetModuleQualifiedPath(), GetParameters(selected.Description, selected.Groups), instance, action);
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
                        string.Format(CultureInfo.InvariantCulture, @"(?<{0}>[/A-Z0-9._-]*)", segment.GetParameterName());

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