namespace Nancy.Routing
{
    public class RoutePatternMatchResult : IRoutePatternMatchResult
    {
        public RoutePatternMatchResult(bool isMatch, DynamicDictionary parameters)
        {
            this.IsMatch = isMatch;
            this.Parameters = parameters;
        }

        public bool IsMatch { get; private set; }
        
        public DynamicDictionary Parameters { get; private set; }
    }
}