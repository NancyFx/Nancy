namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

        public Route Resolve(Request request, IRouteCache cache)
        {
            return new FakeRoute();
        }
    }
}
