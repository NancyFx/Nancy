namespace Nancy.Routing.Constraints
{
    using System.Linq;

    public class AlphaRouteConstraint : RouteConstraintBase<string>
    {
        public override string Name
        {
            get { return "alpha"; }
        }

        protected override bool TryMatch(string constraint, string segment, out string matchedValue)
        {
            if (!segment.All(char.IsLetter))
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}