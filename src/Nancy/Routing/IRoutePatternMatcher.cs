namespace Nancy.Routing
{
    public interface IRoutePatternMatcher
    {
        IRoutePatternMatchResult Match(string requestedPath, string routePath);
    }
}