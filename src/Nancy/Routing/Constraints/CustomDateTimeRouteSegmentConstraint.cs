namespace Nancy.Routing.Constraints
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Constraint for <see cref="DateTime"/> route segments with custom format.
    /// </summary>
    public class CustomDateTimeRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<DateTime>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "datetime"; }
        }

        /// <summary>
        /// Tries to match the given segment and parameters against the constraint.
        /// </summary>
        /// <param name="segment">The segment to match.</param>
        /// <param name="parameters">The parameters to match.</param>
        /// <param name="matchedValue">The matched value.</param>
        /// <returns>
        ///   <c>true</c> if the segment and parameters matches the constraint, <c>false</c> otherwise.
        /// </returns>
        protected override bool TryMatch(string segment, string[] parameters, out DateTime matchedValue)
        {
            return DateTime.TryParseExact(segment,
                parameters[0],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out matchedValue);
        }
    }
}