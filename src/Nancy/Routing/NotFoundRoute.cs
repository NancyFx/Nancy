namespace Nancy.Routing
{
    public class NotFoundRoute : Route
    {
        public NotFoundRoute(string method, string path)
            : base(method, -1, path, null, x => new NotFoundResponse(), new DynamicDictionary())
        {
        }
    }
}