namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;

    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;
    using ResolveResult = System.Tuple<Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    /// <summary>
    /// The default implementation for deciding if any of the available routes is a match for the incoming HTTP request.
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog nancyModuleCatalog;
        private readonly IRoutePatternMatcher routePatternMatcher;
        private readonly INancyModuleBuilder moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteResolver"/> class.
        /// </summary>
        /// <param name="nancyModuleCatalog">The module catalog that modules should be</param>
        /// <param name="routePatternMatcher">The route pattern matcher that should be used to verify if the route is a match to any of the registered routes.</param>
        /// <param name="moduleBuilder">The module builder that will make sure that the resolved module is full configured.</param>
        public DefaultRouteResolver(INancyModuleCatalog nancyModuleCatalog, IRoutePatternMatcher routePatternMatcher, INancyModuleBuilder moduleBuilder)
        {
            this.nancyModuleCatalog = nancyModuleCatalog;
            this.routePatternMatcher = routePatternMatcher;
            this.moduleBuilder = moduleBuilder;
        }

        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="routeCache">Route cache</param>
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary, Item3 being the prereq, Item4 being the postreq</returns>
        public ResolveResult Resolve(NancyContext context, IRouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[RouteResolver] No Routes Available"));
                return new ResolveResult(new NotFoundRoute(context.Request.Method, context.Request.Path), DynamicDictionary.Empty, null, null);
            }

            var routesThatMatchRequestedPath = this.GetRoutesThatMatchRequestedPath(routeCache, context).ToList();

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[RouteResolver] No Matching Routes Available"));
                return new ResolveResult(new NotFoundRoute(context.Request.Method, context.Request.Path), DynamicDictionary.Empty, null, null);
            }

            var routesWithCorrectRequestMethod =
                GetRoutesWithCorrectRequestMethod(context.Request, routesThatMatchRequestedPath).ToList();

            if (NoRoutesWereForTheRequestedMethod(routesWithCorrectRequestMethod))
            {
                var allowedMethods = routesThatMatchRequestedPath.Select(x => x.Item3.Method);
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[RouteResolver] Route Matched But Method Not Allowed"));
                return new ResolveResult(new MethodNotAllowedRoute(context.Request.Path, context.Request.Method, allowedMethods), DynamicDictionary.Empty, null, null);
            }

            var exactMatch = GetRouteMatchesWithExactPathMatch(routesWithCorrectRequestMethod).FirstOrDefault();
            if (exactMatch != null)
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[RouteResolver] Found exact match route"));

                return this.CreateRouteAndParametersFromMatch(context, exactMatch);
            }

            var routeMatchesWithMostParameterCaptures = 
                GetTopRouteMatches(routesWithCorrectRequestMethod);

            var routeMatchToReturn = 
                GetSingleRouteToReturn(routeMatchesWithMostParameterCaptures);

            context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[RouteResolver] Selected best match"));

            return this.CreateRouteAndParametersFromMatch(context, routeMatchToReturn);
        }

        private ResolveResult CreateRouteAndParametersFromMatch(NancyContext context, RouteCandidate routeMatchToReturn)
        {
            var associatedModule =
                this.GetInitializedModuleForMatch(context, routeMatchToReturn);

            var route = associatedModule.Routes.ElementAt(routeMatchToReturn.Item2);

            return new ResolveResult(route, routeMatchToReturn.Item4.Parameters, associatedModule.Before, associatedModule.After);
        }

        private NancyModule GetInitializedModuleForMatch(NancyContext context, RouteCandidate routeMatchToReturn)
        {
            var module =
                this.nancyModuleCatalog.GetModuleByKey(routeMatchToReturn.Item1, context);

            return this.moduleBuilder.BuildModule(module, context);
        }

        private static RouteCandidate GetSingleRouteToReturn(IEnumerable<RouteCandidate> routesWithMostParameterCaptures)
        {
            return routesWithMostParameterCaptures.First();
        }

        private static IEnumerable<RouteCandidate> GetRouteMatchesWithExactPathMatch(IEnumerable<RouteCandidate> routesWithCorrectRequestMethod)
        {
            return routesWithCorrectRequestMethod.Where(
                x => x.Item4.Parameters.GetDynamicMemberNames().Count() == 0);
        }

        private static IEnumerable<RouteCandidate> GetTopRouteMatches(IEnumerable<RouteCandidate> routesWithCorrectRequestMethod)

        {
            var maxSegments = 0;
            var maxParameters = 0;
            // Order is by number of path segment matches first number of parameter matches second.  
            // If two candidates have the same number of path segments the tie breaker is the parameter count.
            foreach (var tuple in routesWithCorrectRequestMethod
                .OrderBy(x => x.Item4.Parameters.GetDynamicMemberNames().Count())
                .OrderByDescending(x => x.Item3.Path.Count(c => c.Equals('/'))))
            {
                var segments = tuple.Item3.Path.Count(c => c == '/');
                var parameters = tuple.Item4.Parameters.GetDynamicMemberNames().Count();
                if (segments < maxSegments || parameters < maxParameters)
                    yield break;
                maxSegments = segments;
                maxParameters = parameters;
                yield return tuple;
            }
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
                   let result = this.routePatternMatcher.Match(context.Request.Path, routeDescription.Path)
                   where result.IsMatch
                   select new RouteCandidate(cacheEntry.Key, routeIndex, routeDescription, result);
        }
    }
}
