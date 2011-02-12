namespace Nancy.Routing
{
    using System;

    public interface IRouteResolver
    {
        Tuple<Route, DynamicDictionary> Resolve(Request request, RouteCache cache);
    }
}