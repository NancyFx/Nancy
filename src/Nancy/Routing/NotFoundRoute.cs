namespace Nancy.Routing
{
    public class NotFoundRoute : Route
    {
        public NotFoundRoute(string route)
            : base(route, null, null, x => new NotFoundResponse())
        {
        }
    }
}