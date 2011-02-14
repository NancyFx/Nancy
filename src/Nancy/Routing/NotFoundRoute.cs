namespace Nancy.Routing
{
    public class NotFoundRoute : Route
    {
        public NotFoundRoute(string method, string path)
            : base(method, path, null, x => new NotFoundResponse())
        {
        }
    }
}