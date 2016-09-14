namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="long"/> route segments.
    /// </summary>
    public class LongRouteSegmentConstraint : RouteSegmentConstraintBase<long>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "long"; }
        }

        /// <summary>
        /// Tries to match the given segment against the constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="segment">The segment to match.</param>
        /// <param name="matchedValue">The matched value.</param>
        /// <returns>
        /// <see langword="true"/> if the segment matches the constraint, <see langword="false"/> otherwise.
        /// </returns>
        protected override bool TryMatch(string constraint, string segment, out long matchedValue)
        {
            return long.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}