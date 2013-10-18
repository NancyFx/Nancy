namespace Nancy.Routing.Constraints
{
    public class LengthRouteConstraint : ParameterizedRouteConstraint<string>
    {
        public override string Name
        {
            get { return "length"; }
        }

        protected override bool TryMatch(string segment, string[] parameters, out string matchedValue)
        {
            int minLength;
            int maxLength;

            if (parameters.Length == 2)
            {
                if (!this.TryParseInt(parameters[0], out minLength) ||
                    !this.TryParseInt(parameters[1], out maxLength))
                {
                    matchedValue = null;
                    return false;
                }
            }
            else if (parameters.Length == 1)
            {
                minLength = 0;

                if (!this.TryParseInt(parameters[0], out maxLength))
                {
                    matchedValue = null;
                    return false;
                }
            }
            else
            {
                matchedValue = null;
                return false;
            }

            if (segment.Length < minLength || segment.Length > maxLength)
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}