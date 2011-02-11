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

        public Route Resolve(Request request, RouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                return new NotFoundRoute(request.Uri, request.Method);
            }

            var routesThatMatchRequestedPath = 
                GetRoutesThatMatchRequestedPath(routeCache, request);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new NotFoundRoute(request.Uri, request.Method);
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

        private Route CreateRouteFromMatch(Request request, Tuple<RouteDescription, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var associatedModule = 
                GetInitializedModuleForMatch(request, routeMatchToReturn);

            var actionToInvokeForRoute =
                associatedModule.GetRoutes(routeMatchToReturn.Item1.Method).GetRoute(
                    routeMatchToReturn.Item1.Path).Action;

            return new Route(request.Uri, routeMatchToReturn.Item2.Parameters, associatedModule, actionToInvokeForRoute);
        }

        private NancyModule GetInitializedModuleForMatch(Request request, Tuple<RouteDescription, IRoutePatternMatchResult> routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1.ModuleKey);

            module.Request = request;
            module.TemplateEngineSelector = this.templateEngineSelector;

            return module;
        }

        private static Tuple<RouteDescription, IRoutePatternMatchResult> GetSingleRouteToReturn(IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> routesWithMostParameterCaptures)
        {
            return routesWithMostParameterCaptures.First();
        }

        private static IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> GetRouteMatchesWithMostParameterCaptures(IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            var maxParameterCount =
                routesWithCorrectRequestMethod.Max(x => x.Item2.Parameters.GetDynamicMemberNames().Count());

            return routesWithCorrectRequestMethod.Where(
                x => x.Item2.Parameters.GetDynamicMemberNames().Count() == maxParameterCount);
        }

        private static bool NoRoutesWereForTheRequestedMethod(IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> routesWithCorrectRequestMethod)
        {
            return !routesWithCorrectRequestMethod.Any();
        }

        private static IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> GetRoutesWithCorrectRequestMethod(Request request, IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return  from route in routesThatMatchRequestedPath
                    let routeMethod = route.Item1.Method.ToUpperInvariant()
                    let requestMethod = request.Method.ToUpperInvariant()
                    where routeMethod.Equals(requestMethod) || (routeMethod.Equals("GET") && requestMethod.Equals("HEAD"))                    
                    select route;
        }

        private static bool NoRoutesWereAbleToBeMatchedInRouteCache(IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> routesThatMatchRequestedPath)
        {
            return !routesThatMatchRequestedPath.Any();
        }

        private IEnumerable<Tuple<RouteDescription, IRoutePatternMatchResult>> GetRoutesThatMatchRequestedPath(IEnumerable<RouteDescription> routeCache, Request request)
        {
            return from x in routeCache
                   let result = this.routePatternMatcher.Match(request.Uri, x.Path)
                   where result.IsMatch
                   select new Tuple<RouteDescription, IRoutePatternMatchResult>(x, result);
        }
    }
}