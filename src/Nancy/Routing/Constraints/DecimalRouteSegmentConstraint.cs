namespace Nancy.Routing.Constraints
{
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="decimal"/> route segments.
    /// </summary>
    public class DecimalRouteSegmentConstraint : RouteSegmentConstraintBase<decimal>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "decimal"; }
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
        protected override bool TryMatch(string constraint, string segment, out decimal matchedValue)
        {
            return decimal.TryParse(segment, NumberStyles.Number, CultureInfo.InvariantCulture, out matchedValue);
        }
    }
}