namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.ViewEngines;
    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;

    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;
    using ResolveResult = System.Tuple<Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    /// <summary>
    /// The default implementation for deciding if any of the available routes is a match for the incoming HTTP request.
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog nancyModuleCatalog;
        private readonly IRoutePatternMatcher routePatternMatcher;
<<<<<<< HEAD
        private readonly IViewFactory viewFactory;

        public DefaultRouteResolver(INancyModuleCatalog nancyModuleCatalog, IRoutePatternMatcher routePatternMatcher, IViewFactory viewFactory)
        {
            this.nancyModuleCatalog = nancyModuleCatalog;
            this.routePatternMatcher = routePatternMatcher;
            this.viewFactory = viewFactory;
        }

        public Tuple<Route, DynamicDictionary> Resolve(Request request, IRouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(request.Method, request.Uri), DynamicDictionary.Empty);
            }

            var routesThatMatchRequestedPath = this.GetRoutesThatMatchRequestedPath(routeCache, request);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new Tuple<Route, DynamicDictionary>(new NotFoundRoute(request.Method, request.Uri), DynamicDictionary.Empty);
=======
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
                return new ResolveResult(new NotFoundRoute(context.Request.Method, context.Request.Uri), DynamicDictionary.Empty, null, null);
            }

            var routesThatMatchRequestedPath = this.GetRoutesThatMatchRequestedPath(routeCache, context);

            if (NoRoutesWereAbleToBeMatchedInRouteCache(routesThatMatchRequestedPath))
            {
                return new ResolveResult(new NotFoundRoute(context.Request.Method, context.Request.Uri), DynamicDictionary.Empty, null, null);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
            }

            var routesWithCorrectRequestMethod =
                GetRoutesWithCorrectRequestMethod(context.Request, routesThatMatchRequestedPath);

            if (NoRoutesWereForTheRequestedMethod(routesWithCorrectRequestMethod))
            {
<<<<<<< HEAD
                return new Tuple<Route, DynamicDictionary>(new MethodNotAllowedRoute(request.Uri, request.Method), DynamicDictionary.Empty);
=======
                return new ResolveResult(new MethodNotAllowedRoute(context.Request.Uri, context.Request.Method), DynamicDictionary.Empty, null, null);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
            }

            var routeMatchesWithMostParameterCaptures = 
                GetRouteMatchesWithMostParameterCaptures(routesWithCorrectRequestMethod);

            var routeMatchToReturn = 
                GetSingleRouteToReturn(routeMatchesWithMostParameterCaptures);

<<<<<<< HEAD
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
            module.View = viewFactory;
=======
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
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147

            return this.moduleBuilder.BuildModule(module, context);
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

<<<<<<< HEAD
        private IEnumerable<RouteCandidate> GetRoutesThatMatchRequestedPath(IRouteCache routeCache, Request request)
=======
        private IEnumerable<RouteCandidate> GetRoutesThatMatchRequestedPath(IRouteCache routeCache, NancyContext context)
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
        {
            return from cacheEntry in routeCache
                   from cacheEntryRoutes in cacheEntry.Value
                   let routeIndex = cacheEntryRoutes.Item1
                   let routeDescription = cacheEntryRoutes.Item2
<<<<<<< HEAD
                   where ((routeDescription.Condition == null) || (routeDescription.Condition(request)))
                   let result = this.routePatternMatcher.Match(request.Uri, routeDescription.Path)
=======
                   where ((routeDescription.Condition == null) || (routeDescription.Condition(context)))
                   let result = this.routePatternMatcher.Match(context.Request.Uri, routeDescription.Path)
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
                   where result.IsMatch
                   select new RouteCandidate(cacheEntry.Key, routeIndex, routeDescription, result);
        }
    }
}
