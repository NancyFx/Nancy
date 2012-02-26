namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;
    using ResolveResult = System.Tuple<Routing.Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

        ResolveResult IRouteResolver.Resolve(NancyContext context)
        {
            return new ResolveResult(new FakeRoute(), new DynamicDictionary(), null, null);
        }
    }
}
