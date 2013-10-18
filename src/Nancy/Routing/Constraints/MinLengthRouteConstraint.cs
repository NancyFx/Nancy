namespace Nancy.Routing.Constraints
{
    public class MinLengthRouteConstraint : ParameterizedRouteConstraint<string>
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