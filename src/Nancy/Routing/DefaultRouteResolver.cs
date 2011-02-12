namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog nancyModuleCatalog;
        private readonly IRoutePatternMatcher routePatternMatcher;
        private readonly ITemplateEngineSelector templateEngineSelector;

        public DefaultRouteResolver(INancyModuleCatalog nancyModuleCatalog, IRoutePatternMatcher routePatternMatcher, ITemplateEngineSelector templateEngineSelector)
        {
            this.nancyModuleCatalog = nancyModuleCatalog;
            this.routePatternMatcher = routePatternMatcher;
            this.templateEngineSelector = templateEngineSelector;
        }

        public Tuple<Route, DynamicDictionary> Resolve(Request request, RouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(request.Uri, request.Method), DynamicDictionary.Empty);
            }

            var routesThatMatchRequestedPath = this.GetRoutesThatMatchRequestedPath(routeCache, request);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(request.Uri, request.Method), DynamicDictionary.Empty);
            }

            var routesWithCorrectRequestMethod = 
                GetRoutesWithCorrectRequestMethod(request, routesThatMatchRequestedPath);

            if (NoRoutesWereForTheRequestedMethod(routesWithCorrectRequestMethod))
            {
                return new Tuple<Route, DynamicDictionary>(new MethodNotAllowedRoute(request.Uri, request.Method), DynamicDictionary.Empty);
            }

            var routeMatchesWithMostParameterCaptures = 
                GetRouteMatchesWithMostParameterCaptures(routesWithCorrectRequestMethod);

            var routeMatchToReturn = 
                GetSingleRouteToReturn(routeMatchesWithMostParameterCaptures);

            return this.CreateRouteAndParametersFromMatch(request, routeMatchToReturn);
        }

        private Tuple<Route, DynamicDictionary> CreateRouteAndParametersFromMatch(Request request, Tuple<string, RouteDescription, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var associatedModule =
                this.GetInitializedModuleForMatch(request, routeMatchToReturn);

            var route = associatedModule.GetRoutes(routeMatchToReturn.Item2.Method).GetRouteByIndex(routeMatchToReturn.Item2.Index);

            return new Tuple<Route, DynamicDictionary>(route, routeMatchToReturn.Item3.Parameters);
        }

        private NancyModule GetInitializedModuleForMatch(Request request, Tuple<string, RouteDescription, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1);

            module.Request = request;
            module.TemplateEngineSelector = this.templateEngineSelector;

            return module;
        }

        private static Tuple<string, RouteDescription, IRoutePatternMatchResult> GetSingleRouteToReturn(IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> routesWithMostParameterCaptures)
        {
            return routesWithMostParameterCaptures.First();
        }

        private static IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> GetRouteMatchesWithMostParameterCaptures(IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            var maxParameterCount =
                routesWithCorrectRequestMethod.Max(x => x.Item3.Parameters.GetDynamicMemberNames().Count());

            return routesWithCorrectRequestMethod.Where(
                x => x.Item3.Parameters.GetDynamicMemberNames().Count() == maxParameterCount);
        }

        private static bool NoRoutesWereForTheRequestedMethod(IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            return !routesWithCorrectRequestMethod.Any();
        }

        private static IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> GetRoutesWithCorrectRequestMethod(Request request, IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return from route in routesThatMatchRequestedPath
                   let routeMethod = route.Item2.Method.ToUpperInvariant()
                   let requestMethod = request.Method.ToUpperInvariant()
                   where routeMethod.Equals(requestMethod) || (routeMethod.Equals("GET") && requestMethod.Equals("HEAD"))                    
                   select route;
        }

        private static bool NoRoutesWereAbleToBeMatchedInRouteCache(IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return !routesThatMatchRequestedPath.Any();
        }

        private IEnumerable<Tuple<string, RouteDescription, IRoutePatternMatchResult>> GetRoutesThatMatchRequestedPath(RouteCache routeCache, Request request)
        {
            return from cacheEntry in routeCache
                   from routeDescription in cacheEntry.Value
                   let result = this.routePatternMatcher.Match(request.Uri, routeDescription.Path)
                   where result.IsMatch
                   select new Tuple<string, RouteDescription, IRoutePatternMatchResult>(cacheEntry.Key, routeDescription, result);
        }
    }
}