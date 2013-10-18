namespace Nancy.Routing.Constraints
{
    public class MaxRouteConstraint : ParameterizedRouteConstraint<int>
    {
        public override string Name
        {
            get { return "max"; }
        }

        protected override bool TryMatch(string segment, string[] parameters, out int matchedValue)
        {
            int minValue;
            int intValue;

            if (!this.TryParseInt(parameters[0], out minValue) ||
                !this.TryParseInt(segment, out intValue))
            {
                matchedValue = default(int);
                return false;
            }

            if (intValue > minValue)
            {
                matchedValue = default(int);
                return false;
            }

            matchedValue = intValue;
            return true;
        }
    }
}