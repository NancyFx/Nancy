namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Helpers;
    using Trie;

    /// <summary>
    /// Default implementation of the <see cref="IRouteResolver"/> interface.
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog catalog;
        private readonly INancyModuleBuilder moduleBuilder;
        private readonly IRouteCache routeCache;
        private readonly IRouteResolverTrie trie;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteResolver"/> class, using
        /// the provided <paramref name="catalog"/>, <paramref name="moduleBuilder"/>,
        /// <paramref name="routeCache"/> and <paramref name="trie"/>.
        /// </summary>
        /// <param name="catalog">A <see cref="INancyModuleCatalog"/> instance.</param>
        /// <param name="moduleBuilder">A <see cref="INancyModuleBuilder"/> instance.</param>
        /// <param name="routeCache">A <see cref="IRouteCache"/> instance.</param>
        /// <param name="trie">A <see cref="IRouteResolverTrie"/> instance.</param>
        public DefaultRouteResolver(INancyModuleCatalog catalog, INancyModuleBuilder moduleBuilder, IRouteCache routeCache, IRouteResolverTrie trie)
        {
            this.catalog = catalog;
            this.moduleBuilder = moduleBuilder;
            this.routeCache = routeCache;
            this.trie = trie;

            this.BuildTrie();
        }

        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>A <see cref="ResolveResult"/> containing the resolved route information.</returns>
        public ResolveResult Resolve(NancyContext context)
        {
            var pathDecoded =
                HttpUtility.UrlDecode(context.Request.Path);

            var results = this.trie.GetMatches(GetMethod(context), pathDecoded, context);

            if (!results.Any())
            {
                var allowedMethods =
                    this.trie.GetOptions(pathDecoded, context).ToArray();

                if (IsOptionsRequest(context))
                {
                    return BuildOptionsResult(allowedMethods, context);
                }

                return IsMethodNotAllowed(allowedMethods) ?
                    BuildMethodNotAllowedResult(context, allowedMethods) :
                    GetNotFoundResult(context);
            }

            // Sort in descending order
            Array.Sort(results, (m1, m2) => -m1.CompareTo(m2));

            for (var index = 0; index < results.Length; index++)
            {
                var matchResult = results[index];
                if (matchResult.Condition == null || matchResult.Condition.Invoke(context))
                {
                    return this.BuildResult(context, matchResult);
                }
            }

            return GetNotFoundResult(context);
        }

        private static ResolveResult BuildMethodNotAllowedResult(NancyContext context, IEnumerable<string> allowedMethods)
        {
            var route =
                new MethodNotAllowedRoute(context.Request.Path, context.Request.Method, allowedMethods);

            return new ResolveResult(route, new DynamicDictionary(), null, null, null);
        }

        private static bool IsMethodNotAllowed(IEnumerable<string> allowedMethods)
        {
            return allowedMethods.Any() && !StaticConfiguration.DisableMethodNotAllowedResponses;
        }

        private static bool IsOptionsRequest(NancyContext context)
        {
            return context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase);
        }

        private void BuildTrie()
        {
            this.trie.BuildTrie(this.routeCache);
        }

        private static ResolveResult BuildOptionsResult(IEnumerable<string> allowedMethods, NancyContext context)
        {
            var path =
                context.Request.Path;

            var optionsResult =
                new OptionsRoute(path, allowedMethods);

            return new ResolveResult(
                optionsResult,
                new DynamicDictionary(),
                null,
                null,
                null);
        }

        private ResolveResult BuildResult(NancyContext context, MatchResult result)
        {
            var associatedModule = this.GetModuleFromMatchResult(context, result);

            context.NegotiationContext.SetModule(associatedModule);

            var route = associatedModule.Routes.ElementAt(result.RouteIndex);
            var parameters = DynamicDictionary.Create(result.Parameters);

            return new ResolveResult
            {
                Route = route,
                Parameters = parameters,
                Before = associatedModule.Before,
                After = associatedModule.After,
                OnError = associatedModule.OnError
            };
        }

        private INancyModule GetModuleFromMatchResult(NancyContext context, MatchResult result)
        {
            var module =
                this.catalog.GetModule(result.ModuleType, context);

            return this.moduleBuilder.BuildModule(module, context);
        }

        private static ResolveResult GetNotFoundResult(NancyContext context)
        {
            return new ResolveResult
            {
                Route = new NotFoundRoute(context.Request.Method, context.Request.Path),
                Parameters = DynamicDictionary.Empty,
                Before = null,
                After = null,
                OnError = null
            };
        }

        private static string GetMethod(NancyContext context)
        {
            var requestedMethod =
                context.Request.Method;

            if (!StaticConfiguration.EnableHeadRouting)
            {
                return requestedMethod.Equals("HEAD", StringComparison.OrdinalIgnoreCase) ?
                    "GET" :
                    requestedMethod;
            }

            return requestedMethod;
        }
    }
}
