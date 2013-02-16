namespace Nancy.Routing
{
    using System;
    using System.Linq;

    using Nancy;
    using Nancy.Routing.Trie;

    using MatchResult = Nancy.Routing.Trie.MatchResult;

    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly INancyModuleCatalog catalog;

        private readonly INancyModuleBuilder moduleBuilder;

        private readonly IRouteCache routeCache;

        private readonly IRouteResolverTrie trie;

        public DefaultRouteResolver(INancyModuleCatalog catalog, INancyModuleBuilder moduleBuilder, IRouteCache routeCache, IRouteResolverTrie trie)
        {
            this.catalog = catalog;
            this.moduleBuilder = moduleBuilder;
            this.routeCache = routeCache;
            this.trie = trie;

            this.BuildTrie();
        }

        private void BuildTrie()
        {
            this.trie.BuildTrie(this.routeCache);
        }

        public ResolveResult Resolve(NancyContext context)
        {
            if (this.IsOptionsRequest(context))
            {
                return this.BuildOptionsResult(context);
            }

            var results = this.trie.GetMatches(GetMethod(context), context.Request.Path, context);

            if (!results.Any())
            {
                return this.GetNotFoundResult(context);
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

            return this.GetNotFoundResult(context);
        }

        private ResolveResult BuildOptionsResult(NancyContext context)
        {
            var path = context.Request.Path;

            var options = this.trie.GetOptions(path, context);

            var optionsResult = new OptionsRoute(path, options);

            return new ResolveResult(
                            optionsResult,
                            new DynamicDictionary(), 
                            null,
                            null,
                            null);                        
        }

        private bool IsOptionsRequest(NancyContext context)
        {
            return context.Request.Method.Equals("OPTIONS", StringComparison.Ordinal);
        }

        private ResolveResult BuildResult(NancyContext context, MatchResult result)
        {
            var associatedModule = this.GetModuleFromMatchResult(context, result);
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
                this.catalog.GetModuleByKey(result.ModuleKey, context);

            return this.moduleBuilder.BuildModule(module, context);
        }

        private ResolveResult GetNotFoundResult(NancyContext context)
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
            var requestedMethod = context.Request.Method;
            
            return requestedMethod.Equals("HEAD", StringComparison.Ordinal) ? "GET" : requestedMethod;
        }
    }
}
