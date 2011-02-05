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

        public Route Resolve(Request request, IRouteCache routeCache)
        {
            if (RouteCacheIsEmpty(routeCache))
            {
                return new NotFoundRoute(request.Uri);
            }

            var routesThatMatchRequestedPath = 
                GetRoutesThatMatchRequestedPath(routeCache, request);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new NotFoundRoute(request.Uri);
            }

            var routesWithCorrectRequestMethod = 
                GetRoutesWithCorrectRequestMethod(request, routesThatMatchRequestedPath);

            if (NoRoutesWereForTheRequestedMethod(routesWithCorrectRequestMethod))
            {
                return new MethodNotAllowedRoute(request.Uri);
            }

            var routeMatchesWithMostParameterCaptures = 
                GetRouteMatchesWithMostParameterCaptures(routesWithCorrectRequestMethod);

            var routeMatchToReturn = 
                GetSingleRouteToReturn(routeMatchesWithMostParameterCaptures);

            return this.CreateRouteFromMatch(request, routeMatchToReturn);
        }

        private Route CreateRouteFromMatch(Request request, Tuple<RouteCacheEntry, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var associatedModule = 
                GetInitializedModuleForMatch(request, routeMatchToReturn);

            var actionToInvokeForRoute =
                associatedModule.GetRoutes(routeMatchToReturn.Item1.Method).GetRoute(
                    routeMatchToReturn.Item1.Path).Action;

            return new Route(request.Uri, routeMatchToReturn.Item2.Parameters, associatedModule, actionToInvokeForRoute);
        }

        private NancyModule GetInitializedModuleForMatch(Request request, Tuple<RouteCacheEntry, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1.ModuleKey);

            module.Request = request;
            module.TemplateEngineSelector = this.templateEngineSelector;

            return module;
        }

        private static Tuple<RouteCacheEntry, IRoutePatternMatchResult> GetSingleRouteToReturn(IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> routesWithMostParameterCaptures)
        {
            return routesWithMostParameterCaptures.First();
        }

        private static IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> GetRouteMatchesWithMostParameterCaptures(IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            var maxParameterCount =
                routesWithCorrectRequestMethod.Max(x => x.Item2.Parameters.GetDynamicMemberNames().Count());

            return routesWithCorrectRequestMethod.Where(
                x => x.Item2.Parameters.GetDynamicMemberNames().Count() == maxParameterCount);
        }

        private static bool NoRoutesWereForTheRequestedMethod(IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            return !routesWithCorrectRequestMethod.Any();
        }

        private static IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> GetRoutesWithCorrectRequestMethod(Request request, IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return routesThatMatchRequestedPath.Where(x => x.Item1.Method.Equals(request.Method, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private static bool RouteCacheIsEmpty(IEnumerable<RouteCacheEntry> routeCache)
        {
            return !routeCache.Any();
        }

        private static bool NoRoutesWereAbleToBeMatchedInRouteCache(IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return !routesThatMatchRequestedPath.Any();
        }

        private IEnumerable<Tuple<RouteCacheEntry, IRoutePatternMatchResult>> GetRoutesThatMatchRequestedPath(IEnumerable<RouteCacheEntry> routeCache, Request request)
        {
            return from x in routeCache
                   let result = this.routePatternMatcher.Match(request.Uri, x.Path)
                   where result.IsMatch
                   select new Tuple<RouteCacheEntry, IRoutePatternMatchResult>(x, result);
        }
    }
}