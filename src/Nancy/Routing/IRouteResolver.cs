namespace Nancy.Routing
{
    public interface IRouteResolver
    {
        Route Resolve(Request request, IRouteCache cache);
    }
}