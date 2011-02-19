namespace Nancy.Tests.Fakes
{
    using System;
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

        Tuple<Route, DynamicDictionary> IRouteResolver.Resolve(NancyContext context, IRouteCache cache)
        {
            return new Tuple<Route, DynamicDictionary>(new FakeRoute(), new DynamicDictionary());
        }
    }
}
