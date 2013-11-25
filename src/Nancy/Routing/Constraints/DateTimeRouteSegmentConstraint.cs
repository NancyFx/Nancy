namespace Nancy.Routing.Constraints
{
    using System;

    /// <summary>
    /// Constraint for <see cref="DateTime"/> route segments.
    /// </summary>
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