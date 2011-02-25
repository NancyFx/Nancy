namespace Nancy.Tests.Fakes
{
    using System;
    using Nancy.Routing;
    using ResolveResult = System.Tuple<Routing.Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    public class FakeRouteResolver : IRouteResolver
    {
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

<<<<<<< HEAD
        Tuple<Route, DynamicDictionary> IRouteResolver.Resolve(Request request, IRouteCache cache)
        {
            return new Tuple<Route, DynamicDictionary>(new FakeRoute(), new DynamicDictionary());
=======
        ResolveResult IRouteResolver.Resolve(NancyContext context, IRouteCache cache)
        {
            return new ResolveResult(new FakeRoute(), new DynamicDictionary(), null, null);
>>>>>>> 4db860ef0fe65aa3c4b5ed52625c5ec03e369147
        }
    }
}
