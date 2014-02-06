namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for route segments with a minimum length.
    /// </summary>
    public class MinLengthRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<string>
    {
        public override string Name
        {
            get { return "minlength"; }
        }

        protected override bool TryMatch(string segment, string[] parameters, out string matchedValue)
        {
            int minLength;

            if (!this.TryParseInt(parameters[0], out minLength))
            {
                matchedValue = null;
                return false;
            }

            if (segment.Length < minLength)
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}