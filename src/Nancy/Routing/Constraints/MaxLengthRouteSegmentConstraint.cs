namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for route segments with a maximum length.
    /// </summary>
    public class MaxLengthRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<string>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "maxlength"; }
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
        protected override bool TryMatch(string segment, string[] parameters, out string matchedValue)
        {
            int maxLength;

            if (!this.TryParseInt(parameters[0], out maxLength))
            {
                matchedValue = null;
                return false;
            }

            if (segment.Length > maxLength)
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}