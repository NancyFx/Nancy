namespace Nancy.Routing.Constraints
{
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