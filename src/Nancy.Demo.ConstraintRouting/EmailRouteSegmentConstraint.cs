namespace Nancy.Demo.ModelBinding
{
    using Nancy.Routing.Constraints;

    public class EmailRouteSegmentConstraint : RouteSegmentConstraintBase<string>
    {
        public override string Name
        {
            get { return "email"; }
        }

        protected override bool TryMatch(string constraint, string segment, out string matchedValue)
        {
            // Using @jchannon logic for validating e-mail address
            if (segment.Contains("@") && segment.Contains("."))
            {
                matchedValue = segment;
                return true;
            }

            matchedValue = null;
            return false;
        }
    }
}