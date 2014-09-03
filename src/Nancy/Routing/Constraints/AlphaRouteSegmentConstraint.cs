namespace Nancy.Routing.Constraints
{
    using System.Linq;

    /// <summary>
    /// Constraint for alphabetical route segments.
    /// </summary>
    public class AlphaRouteSegmentConstraint : RouteSegmentConstraintBase<string>
    {
        public override string Name
        {
            get { return "alpha"; }
        }

        protected override bool TryMatch(string constraint, string segment, out string matchedValue)
        {
            if (!segment.All(char.IsLetter))
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}