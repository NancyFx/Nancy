namespace Nancy.Routing
{
    public class NoMatchingRouteFoundRoute : Route
    {
        public NoMatchingRouteFoundRoute(string route)
            : base(route, null, null, x => new NotFoundResponse())
        {
        }
    }
}