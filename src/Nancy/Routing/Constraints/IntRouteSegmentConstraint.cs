namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="int"/> route segments.
    /// </summary>
    public class IntRouteSegmentConstraint : RouteSegmentConstraintBase<int>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "int"; }
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
        protected override bool TryMatch(string constraint, string segment, out int matchedValue)
        {
            return int.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}