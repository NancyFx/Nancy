namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="long"/> route segments.
    /// </summary>
    public class LongRouteSegmentConstraint : RouteSegmentConstraintBase<long>
    {
        public override string Name
        {
            get { return "long"; }
        }

        protected override bool TryMatch(string constraint, string segment, out long matchedValue)
        {
            return long.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}