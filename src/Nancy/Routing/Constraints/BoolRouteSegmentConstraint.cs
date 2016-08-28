namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for <see cref="bool"/> route segments.
    /// </summary>
    public class BoolRouteSegmentConstraint : RouteSegmentConstraintBase<bool>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "bool"; }
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
        protected override bool TryMatch(string constraint, string segment, out bool matchedValue)
        {
            return bool.TryParse(segment, out matchedValue);
        }
    }
}