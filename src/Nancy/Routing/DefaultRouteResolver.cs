namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Nancy.Extensions;

    public sealed class DefaultRouteResolver : IRouteResolver
    {
        private readonly IRouteCache _RouteCache;
        private readonly INancyModuleCatalog _ModuleCatalog;
        private readonly ITemplateEngineSelector _TemplateSelector;
        
        /// <summary>
        /// Initializes a new instance of the RouteResolver class.
        /// </summary>
        /// <param name="routeCache">Route cache provider</param>
        /// <param name="moduleCatalog">Module catalog</param>
        /// <param name="templateSelector">Template selector</param>
        public DefaultRouteResolver(IRouteCache routeCache, INancyModuleCatalog moduleCatalog, ITemplateEngineSelector templateSelector)
        {
            _RouteCache = routeCache;
            _ModuleCatalog = moduleCatalog;
            _TemplateSelector = templateSelector;
        }

        public IRoute GetRoute(IRequest request)
        {
            var matchingRoutes =
                from cacheEntry in _RouteCache
                where cacheEntry.Method == request.Method
                let matcher = BuildRegexMatcher(cacheEntry.Path)
                let result = matcher.Match(request.Uri)
                where result.Success
                where ((cacheEntry.Condition == null) || (cacheEntry.Condition(request)))
                select new
                {
                    CacheEntry = cacheEntry,
                    Groups = result.Groups
                };

            var selected = matchingRoutes
                .OrderByDescending(ma => GetSegmentCount(ma.CacheEntry.Path))
                .FirstOrDefault();

            if (selected == null)
                return new NoMatchingRouteFoundRoute(request.Uri);

            var instance = BuildModuleInstance(selected.CacheEntry.ModuleKey, request);
            if (instance == null)
                return new NoMatchingRouteFoundRoute(request.Uri);

            var routeAction = instance.GetRoutes(selected.CacheEntry.Method).GetRoute(selected.CacheEntry.Path).Action;
            if (routeAction == null)
                return new NoMatchingRouteFoundRoute(request.Uri);

            return new Route(selected.CacheEntry.Path, GetParameters(selected.CacheEntry.Path, selected.Groups), instance, routeAction); 
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
                path.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);

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

        private static int GetSegmentCount(string path)
        {
            var moduleQualifiedPath =
                path;

            var indexOfFirstParameter =
                moduleQualifiedPath.IndexOf('{');

            if (indexOfFirstParameter > -1)
                moduleQualifiedPath = moduleQualifiedPath.Substring(0, indexOfFirstParameter);

            return moduleQualifiedPath.Split('/').Count();
        }

        private NancyModule BuildModuleInstance(string moduleKey, IRequest request)
        {
            var module = _ModuleCatalog.GetModuleByKey(moduleKey);

            module.Request = request;
            module.TemplateEngineSelector = _TemplateSelector;

            return module;
        }
    }
}