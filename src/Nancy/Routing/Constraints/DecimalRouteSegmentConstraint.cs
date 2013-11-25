namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="decimal"/> route segments.
    /// </summary>
    public class DecimalRouteSegmentConstraint : RouteSegmentConstraintBase<decimal>
    {
        public override string Name
        {
            get { return "decimal"; }
        }

        protected override bool TryMatch(string constraint, string segment, out decimal matchedValue)
        {
            return decimal.TryParse(segment, NumberStyles.Number, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}