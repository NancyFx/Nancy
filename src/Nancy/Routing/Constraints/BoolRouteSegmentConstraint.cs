namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for <see cref="bool"/> route segments.
    /// </summary>
    public class BoolRouteSegmentConstraint : RouteSegmentConstraintBase<bool>
    {
        public override string Name
        {
            get { return "bool"; }
        }

        protected override bool TryMatch(string constraint, string segment, out bool matchedValue)
        {
            return bool.TryParse(segment, out matchedValue);
        }
    }
}