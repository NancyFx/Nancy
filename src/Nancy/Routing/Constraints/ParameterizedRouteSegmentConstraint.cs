namespace Nancy.Routing.Constraints
{
    using System.Globalization;
    using System.Linq;

    public abstract class ParameterizedRouteSegmentConstraint<T> : RouteSegmentConstraintBase<T>
    {
        public override bool Matches(string constraint)
        {
            return constraint.Contains('(') && constraint.Contains(')') && base.Matches(constraint.Substring(0, constraint.IndexOf('(')));
        }

        protected override bool TryMatch(string constraint, string segment, out T matchedValue)
        {
            var parameters = constraint.Substring(constraint.IndexOf('(')).Trim('(', ')').Split(',');

            return TryMatch(segment, parameters, out matchedValue);
        }

        protected bool TryParseInt(string s, out int result)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        protected abstract bool TryMatch(string segment, string[] parameters, out T matchedValue);
    }
}