namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="int"/> route segments.
    /// </summary>
    public class IntRouteSegmentConstraint : RouteSegmentConstraintBase<int>
    {
        public override string Name
        {
            get { return "int"; }
        }

        protected override bool TryMatch(string constraint, string segment, out int matchedValue)
        {
            return int.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}