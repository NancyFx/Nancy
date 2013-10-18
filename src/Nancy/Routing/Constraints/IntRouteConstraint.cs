namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    public class IntRouteConstraint : RouteConstraintBase<long>
    {
        public override string Name
        {
            get { return "int"; }
        }

        protected override bool TryMatch(string constraint, string segment, out long matchedValue)
        {
            return long.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}