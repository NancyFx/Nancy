namespace Nancy.Routing
{
    using System.Threading.Tasks;

    /// <summary>
    /// Route that is returned when the path could not be matched.
    /// </summary>
    /// <remarks>This is equal to sending back the 404 HTTP status code.</remarks>
    public class NotFoundRoute : Route<Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundRoute"/> type, for the
        /// specified <paramref name="path"/> and <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The HTTP method of the route.</param>
        /// <param name="path">The path of the route.</param>
        public NotFoundRoute(string method, string path)
            : base(method, path, null, (x,c) => Task.FromResult<Response>(new NotFoundResponse()))
        {
        }
    }
}