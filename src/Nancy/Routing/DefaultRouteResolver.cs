namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;

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

        public Tuple<Route, DynamicDictionary> Resolve(Request request, IRouteCache routeCache)
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

        private Tuple<Route, DynamicDictionary> CreateRouteAndParametersFromMatch(Request request, RouteCandidate routeMatchToReturn)
        {
            var associatedModule =
                this.GetInitializedModuleForMatch(request, routeMatchToReturn);

            var route = associatedModule.Routes.ElementAt(routeMatchToReturn.Item2);

            return new Tuple<Route, DynamicDictionary>(route, routeMatchToReturn.Item4.Parameters);
        }

        private NancyModule GetInitializedModuleForMatch(Request request, RouteCandidate routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1);

            module.Request = request;
            module.TemplateEngineSelector = this.templateEngineSelector;

            return module;
        }

        private static RouteCandidate GetSingleRouteToReturn(IEnumerable<RouteCandidate> routesWithMostParameterCaptures)
        {
            return routesWithMostParameterCaptures.First();
        }

        private static IEnumerable<RouteCandidate> GetRouteMatchesWithMostParameterCaptures(IEnumerable<RouteCandidate> routesWithCorrectRequestMethod)
        {
            var maxParameterCount =
                routesWithCorrectRequestMethod.Max(x => x.Item4.Parameters.GetDynamicMemberNames().Count());

            return routesWithCorrectRequestMethod.Where(
                x => x.Item4.Parameters.GetDynamicMemberNames().Count() == maxParameterCount);
        }

        private static bool NoRoutesWereForTheRequestedMethod(IEnumerable<RouteCandidate> routesWithCorrectRequestMethod)
        {
            return !routesWithCorrectRequestMethod.Any();
        }

        private static IEnumerable<RouteCandidate> GetRoutesWithCorrectRequestMethod(Request request, IEnumerable<RouteCandidate> routesThatMatchRequestedPath)
        {
            return from route in routesThatMatchRequestedPath
                   let routeMethod = route.Item3.Method.ToUpperInvariant()
                   let requestMethod = request.Method.ToUpperInvariant()
                   where routeMethod.Equals(requestMethod) || (routeMethod.Equals("GET") && requestMethod.Equals("HEAD"))                    
                   select route;
        }

        private static bool NoRoutesWereAbleToBeMatchedInRouteCache(IEnumerable<RouteCandidate> routesThatMatchRequestedPath)
        {
            return !routesThatMatchRequestedPath.Any();
        }

        private IEnumerable<RouteCandidate> GetRoutesThatMatchRequestedPath(IRouteCache routeCache, Request request)
        {
            return from cacheEntry in routeCache
                   from cacheEntryRoutes in cacheEntry.Value
                   let routeIndex = cacheEntryRoutes.Item1
                   let routeDescription = cacheEntryRoutes.Item2
                   let result = this.routePatternMatcher.Match(request.Uri, routeDescription.Path)
                   where result.IsMatch
                   select new RouteCandidate(cacheEntry.Key, routeIndex, routeDescription, result);
        }
    }
}