namespace Nancy.Routing.Constraints
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="DateTime"/> route segments with custom format.
    /// </summary>
    public class CustomDateTimeRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<DateTime>
    {
        public override string Name
        {
            get { return "datetime"; }
        }

        protected override bool TryMatch(string segment, string[] parameters, out DateTime matchedValue)
        {
            return DateTime.TryParseExact(segment,
                parameters[0],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out matchedValue);
        }
    }
}