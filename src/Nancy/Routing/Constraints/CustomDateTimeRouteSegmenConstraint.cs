namespace Nancy.Routing.Constraints
{
    using System;
    using System.Globalization;

    public class CustomDateTimeRouteSegmenConstraint : ParameterizedRouteSegmentConstraintBase<DateTime>
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