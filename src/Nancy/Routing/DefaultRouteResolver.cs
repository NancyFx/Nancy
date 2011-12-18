namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diagnostics;
    using RouteCandidate = System.Tuple<string, int, RouteDescription, IRoutePatternMatchResult>;
    using ResolveResult = System.Tuple<Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    /// <summary>
    /// The default implementation for deciding if any of the available routes is a match for the incoming HTTP request.
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver, IDiagnosticsProvider
    {
        private readonly INancyModuleCatalog nancyModuleCatalog;
        private readonly IRoutePatternMatcher routePatternMatcher;
        private readonly INancyModuleBuilder moduleBuilder;
        private readonly IRouteCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteResolver"/> class.
        /// </summary>
        /// <param name="nancyModuleCatalog">The module catalog that modules should be</param>
        /// <param name="routePatternMatcher">The route pattern matcher that should be used to verify if the route is a match to any of the registered routes.</param>
        /// <param name="moduleBuilder">The module builder that will make sure that the resolved module is full configured.</param>
        /// <param name="cache">The route cache that should be used to resolve modules from.</param>
        public DefaultRouteResolver(INancyModuleCatalog nancyModuleCatalog, IRoutePatternMatcher routePatternMatcher, INancyModuleBuilder moduleBuilder, IRouteCache cache)
        {
            this.nancyModuleCatalog = nancyModuleCatalog;
            this.routePatternMatcher = routePatternMatcher;
            this.moduleBuilder = moduleBuilder;
            this.cache = cache;
        }

        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="context">Current context</param>
        
        /// <returns>Tuple - Item1 being the Route, Item2 being the parameters dictionary, Item3 being the prereq, Item4 being the postreq</returns>
        public ResolveResult Resolve(NancyContext context)
        {
            var result =
                this.Resolve(context.Request.Path, context, this.cache);

            return result.Selected;
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

        private static IEnumerable<RouteCandidate> GetTopRouteMatchesNew(Tuple<List<RouteCandidate>, Dictionary<string, List<RouteCandidate>>> routes)
        {
            var maxSegments = 0;
            var maxParameters = 0;
            
            // Order is by number of path segment matches first number of parameter matches second.  
            // If two candidates have the same number of path segments the tie breaker is the parameter count.
            var selectedRoutes = routes.Item1
                .OrderBy(x => x.Item4.Parameters.GetDynamicMemberNames().Count())
                .OrderByDescending(x => x.Item3.Path.Count(c => c.Equals('/')));

            foreach (var tuple in selectedRoutes)
            {
                var segments = 
                    tuple.Item3.Path.Count(c => c == '/');
                
                var parameters = 
                    tuple.Item4.Parameters.GetDynamicMemberNames().Count();
                
                if (segments < maxSegments || parameters < maxParameters)
                {
                    yield break;
                }

                maxSegments = segments;
                maxParameters = parameters;
                
                yield return tuple;
            }
        }

        private ResolveResults Resolve(string path, NancyContext context, IRouteCache routeCache)
        {
            if (routeCache.IsEmpty())
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] No routes available"));
                return new ResolveResults
                {
                    Selected = new ResolveResult(new NotFoundRoute(context.Request.Method, path), DynamicDictionary.Empty, null, null)
                };
            }
            
            var routes =
                routeCache.GetRouteCandidates();

            // Condition
            routes =
                routes.Filter(context, "Invalid condition", (ctx, route) =>{
                    var validCondition =
                        ((route.Item3.Condition == null) || (route.Item3.Condition(ctx)));

                    return new Tuple<bool, RouteCandidate>(
                        validCondition,
                        route
                    );
                });

            if (!routes.Item1.Any())
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] No route had a valid condition"));
                return new ResolveResults
                {
                    Selected = new ResolveResult(new NotFoundRoute(context.Request.Method, path), DynamicDictionary.Empty, null, null),
                    Rejected = routes.Item2
                };
            }

            // Path
            routes =
                routes.Filter(context, "Path did not match", (ctx, route) => {
                    var validationResult = 
                        this.routePatternMatcher.Match(path, route.Item3.Path);

                    var routeToReturn =
                        (validationResult.IsMatch) ? new RouteCandidate(route.Item1, route.Item2, route.Item3, validationResult) : route;

                    return new Tuple<bool, RouteCandidate>(
                        validationResult.IsMatch,
                        routeToReturn
                    );
                });

            if (!routes.Item1.Any())
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] No route matched the requested path"));
                return new ResolveResults
                {
                    Selected = new ResolveResult(new NotFoundRoute(context.Request.Method, path), DynamicDictionary.Empty, null, null),
                    Rejected = routes.Item2
                };
            }

            // Method
            routes =
                routes.Filter(context, "Request method did not match", (ctx, route) =>{
                    var routeMethod = 
                        route.Item3.Method.ToUpperInvariant();

                    var requestMethod = 
                        ctx.Request.Method.ToUpperInvariant();
                    
                    var methodIsValid =
                        routeMethod.Equals(requestMethod) || (routeMethod.Equals("GET") && requestMethod.Equals("HEAD"));

                    return new Tuple<bool, RouteCandidate>(
                        methodIsValid,
                        route
                    );
                });

            if (!routes.Item1.Any())
            {
                var allowedMethods = routes.Item2.Values.SelectMany(x => x.Select(y => y.Item3.Method)).Distinct();
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] Route Matched But Method Not Allowed"));
                return new ResolveResults
                {
                    Selected = new ResolveResult(new MethodNotAllowedRoute(path, context.Request.Method, allowedMethods), DynamicDictionary.Empty, null, null),
                    Rejected = routes.Item2
                };
            }

            // Exact match
            var exactMatchResults =
                routes.Filter(context, "No exact match", (ctx, route) =>{
                    var routeIsExactMatch =
                        !route.Item4.Parameters.GetDynamicMemberNames().Any();

                    return new Tuple<bool, RouteCandidate>(
                        routeIsExactMatch,
                        route
                    );
                });

            if (exactMatchResults.Item1.Any())
            {
                context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] Found exact match route"));
                return new ResolveResults
                {
                    Selected = this.CreateRouteAndParametersFromMatch(context, exactMatchResults.Item1.First()),
                    Rejected = exactMatchResults.Item2
                };
            }

            // First match out of multiple candidates
            var selected =
                GetTopRouteMatchesNew(routes).First();

            context.Diagnostic.TraceLog.WriteLog(s => s.AppendLine("[DefaultRouteResolver] Selected best match"));
            return new ResolveResults
            {
                Selected = this.CreateRouteAndParametersFromMatch(context, selected),
                Rejected = exactMatchResults.Item2
            };
        }

        private class ResolveResults
        {
            public ResolveResults()
            {
                this.Rejected = new Dictionary<string, List<RouteCandidate>>();
            }

            public ResolveResult Selected { get; set; }

            public Dictionary<string, List<RouteCandidate>> Rejected { get; set; }
        }

        public string Name
        {
            get { return "Default route resolver"; }
        }

        public string Description
        {
            get { return "A description"; }
        }

        public object DiagnosticObject
        {
            get { return new DefaultRouteResolverDiagnosticsProvider(this); }
        }

        public class DefaultRouteResolverDiagnosticsProvider
        {
            private readonly DefaultRouteResolver resolver;
            private readonly IRouteCache cache;

            public DefaultRouteResolverDiagnosticsProvider(DefaultRouteResolver resolver)
            {
                this.resolver = resolver;
            }

            //public string ResolveRoute(string method, string path)
            //{
            //    this.resolver.Resolve()
            //}

            private NancyContext CreateContext(string method, string path)
            {
                return new NancyContext();
            }
        }
    }
}