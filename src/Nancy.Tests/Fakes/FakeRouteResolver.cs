namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

        ResolveResult IRouteResolver.Resolve(NancyContext context)
        {
            return new ResolveResult
            {
                Route = new FakeRoute(), 
                Parameters = new DynamicDictionary(), 
                Before = null, 
                After = null, 
                OnError = null
            };
        }
    }
}
