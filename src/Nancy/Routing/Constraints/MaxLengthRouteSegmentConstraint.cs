namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for route segments with a maximum length.
    /// </summary>
    public class MaxLengthRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<string>
    {
        public override string Name
        {
            get { return "maxlength"; }
        }

        protected override bool TryMatch(string segment, string[] parameters, out string matchedValue)
        {
            int maxLength;

            if (!this.TryParseInt(parameters[0], out maxLength))
            {
                matchedValue = null;
                return false;
            }

            if (segment.Length > maxLength)
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}