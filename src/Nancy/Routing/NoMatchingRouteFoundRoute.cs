namespace Nancy.Routing
{
    public class NoMatchingRouteFoundRoute : Route
    {
        public NoMatchingRouteFoundRoute(string route)
            : base(route, null, x => new NotFoundResponse())
        {
        }
    }
}