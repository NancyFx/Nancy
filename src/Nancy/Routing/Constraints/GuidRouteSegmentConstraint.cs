namespace Nancy.Routing.Constraints
{
    using System;

    /// <summary>
    /// Constraint for <see cref="Guid"/> route segments.
    /// </summary>
    public class GuidRouteSegmentConstraint : RouteSegmentConstraintBase<Guid>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "guid"; }
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
        protected override bool TryMatch(string constraint, string segment, out Guid matchedValue)
        {
            return Guid.TryParse(segment, out matchedValue);
        }
    }
}