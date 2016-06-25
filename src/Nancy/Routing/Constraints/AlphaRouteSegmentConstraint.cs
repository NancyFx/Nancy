namespace Nancy.Routing.Constraints
{
    using System.Linq;

    /// <summary>
    /// Constraint for alphabetical route segments.
    /// </summary>
    public class AlphaRouteSegmentConstraint : RouteSegmentConstraintBase<string>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "alpha"; }
        }

        /// <summary>
        /// Tries to match the given segment against the constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="segment">The segment to match.</param>
        /// <param name="matchedValue">The matched value.</param>
        /// <returns>
        ///   <c>true</c> if the segment matches the constraint, <c>false</c> otherwise.
        /// </returns>
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