namespace Nancy.Routing.Constraints
{
    using System;

    public class DateTimeRouteSegmentConstraint : RouteSegmentConstraintBase<DateTime>
    {
        public override string Name
        {
            get { return "datetime"; }
        }

        protected override bool TryMatch(string constraint, string segment, out DateTime matchedValue)
        {
            return DateTime.TryParse(segment, out matchedValue);
        }
    }
}