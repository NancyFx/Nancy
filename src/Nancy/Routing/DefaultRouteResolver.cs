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
            var results = this.trie.GetMatches(context.Request.Method, context.Request.Path, context);

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

        private ResolveResult BuildResult(NancyContext context, MatchResult result)
        {
            var associatedModule = this.GetModuleFromMatchResult(context, result);
            var route = associatedModule.Routes.ElementAt(result.RouteIndex);
            var parameters = DynamicDictionary.Create(result.Parameters);

            return new ResolveResult(
                            route,
                            parameters,
                            associatedModule.Before,
                            associatedModule.After,
                            associatedModule.OnError);
        }

        private INancyModule GetModuleFromMatchResult(NancyContext context, MatchResult result)
        {
            var module =
                this.catalog.GetModuleByKey(result.ModuleKey, context);

            return this.moduleBuilder.BuildModule(module, context);
        }

        private ResolveResult GetNotFoundResult(NancyContext context)
        {
            return new ResolveResult(
                            new NotFoundRoute(context.Request.Method, context.Request.Path),
                            DynamicDictionary.Empty,
                            null,
                            null,
                            null);
        }
    }
}
