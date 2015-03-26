namespace Nancy.Routing.Constraints
{
    using System;

    /// <summary>
    /// Constraint for version route segments.
    /// </summary>
    public class VersionRouteSegmentConstraint : RouteSegmentConstraintBase<Version>
    {
        public override string Name
        {
            get { return "version"; }
        }

        protected override bool TryMatch(string constraint, string segment, out Version matchedValue)
        {
            return Version.TryParse(segment, out matchedValue);
        }
    }
}
