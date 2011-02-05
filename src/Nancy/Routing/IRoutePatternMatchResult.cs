namespace Nancy.Routing
{
    public interface IRoutePatternMatchResult
    {
        bool IsMatch { get; }

        DynamicDictionary Parameters { get; }
    }
}