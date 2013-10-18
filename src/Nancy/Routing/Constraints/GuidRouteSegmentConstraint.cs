namespace Nancy.Routing.Constraints
{
    using System;

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