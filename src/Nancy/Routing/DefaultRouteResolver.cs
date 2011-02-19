namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.ViewEngines;
    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;

    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog nancyModuleCatalog;
        private readonly IRoutePatternMatcher routePatternMatcher;
        private readonly IViewFactory viewFactory;

        public DefaultRouteResolver(INancyModuleCatalog nancyModuleCatalog, IRoutePatternMatcher routePatternMatcher, IViewFactory viewFactory)
        {
            this.nancyModuleCatalog = nancyModuleCatalog;
            this.routePatternMatcher = routePatternMatcher;
            this.viewFactory = viewFactory;
        }

        public Tuple<Route, DynamicDictionary> Resolve(NancyContext context, IRouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(context.Request.Method, context.Request.Uri), DynamicDictionary.Empty);
            }

            var routesThatMatchRequestedPath = this.GetRoutesThatMatchRequestedPath(routeCache, context);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(context.Request.Method, context.Request.Uri), DynamicDictionary.Empty);
            }

            var routesWithCorrectRequestMethod =
                GetRoutesWithCorrectRequestMethod(context.Request, routesThatMatchRequestedPath);

            if (NoRoutesWereForTheRequestedMethod(routesWithCorrectRequestMethod))
            {
                return new Tuple<Route, DynamicDictionary>(new MethodNotAllowedRoute(context.Request.Uri, context.Request.Method), DynamicDictionary.Empty);
            }

            var routeMatchesWithMostParameterCaptures = 
                GetRouteMatchesWithMostParameterCaptures(routesWithCorrectRequestMethod);

            var routeMatchToReturn = 
                GetSingleRouteToReturn(routeMatchesWithMostParameterCaptures);

            return this.CreateRouteAndParametersFromMatch(context, routeMatchToReturn);
        }

        private Tuple<Route, DynamicDictionary> CreateRouteAndParametersFromMatch(NancyContext context, RouteCandidate routeMatchToReturn)
        {
            var associatedModule =
                this.GetInitializedModuleForMatch(context, routeMatchToReturn);

            var route = associatedModule.Routes.ElementAt(routeMatchToReturn.Item2);

            return new Tuple<Route, DynamicDictionary>(route, routeMatchToReturn.Item4.Parameters);
        }

        private NancyModule GetInitializedModuleForMatch(NancyContext context, RouteCandidate routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1, context);

            module.Context = context;
            module.View = viewFactory;

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

        private IEnumerable<RouteCandidate> GetRoutesThatMatchRequestedPath(IRouteCache routeCache, NancyContext context)
        {
            return from cacheEntry in routeCache
                   from cacheEntryRoutes in cacheEntry.Value
                   let routeIndex = cacheEntryRoutes.Item1
                   let routeDescription = cacheEntryRoutes.Item2
                   where ((routeDescription.Condition == null) || (routeDescription.Condition(context)))
                   let result = this.routePatternMatcher.Match(context.Request.Uri, routeDescription.Path)
                   where result.IsMatch
                   select new RouteCandidate(cacheEntry.Key, routeIndex, routeDescription, result);
        }
    }
}
