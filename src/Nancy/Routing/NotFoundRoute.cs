namespace Nancy.Routing
{
    /// <summary>
    /// Route that is returned when the path could not be matched.
    /// </summary>
    /// <remarks>This is equal to sending back the 404 HTTP status code.</remarks>
    public class NotFoundRoute : Route
    {
        public NotFoundRoute(string method, string path)
            : base(method, path, null, x => new NotFoundResponse())
        {
        }
    }
}