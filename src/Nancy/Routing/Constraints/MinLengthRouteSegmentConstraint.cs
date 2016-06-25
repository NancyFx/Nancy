namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for route segments with a minimum length.
    /// </summary>
    public class MinLengthRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<string>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "minlength"; }
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
            int minLength;

            if (!this.TryParseInt(parameters[0], out minLength))
            {
                matchedValue = null;
                return false;
            }

            if (segment.Length < minLength)
            {
                matchedValue = null;
                return false;
            }

            matchedValue = segment;
            return true;
        }
    }
}