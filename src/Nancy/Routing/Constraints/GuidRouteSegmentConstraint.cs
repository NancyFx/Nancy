namespace Nancy.Routing.Constraints
{
    using System;

    /// <summary>
    /// Constraint for <see cref="Guid"/> route segments.
    /// </summary>
    public class GuidRouteSegmentConstraint : RouteSegmentConstraintBase<Guid>
    {
        public override string Name
        {
            get { return "guid"; }
        }

        protected override bool TryMatch(string constraint, string segment, out Guid matchedValue)
        {
            return Guid.TryParse(segment, out matchedValue);
        }
    }
}