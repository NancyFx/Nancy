namespace Nancy.Routing.Constraints
{
    using Nancy.Routing.Trie;

    public interface IRouteConstraint
    {
        bool Matches(string constraint);

        SegmentMatch GetMatch(string constraint, string segment, string parameterName);
    }
}