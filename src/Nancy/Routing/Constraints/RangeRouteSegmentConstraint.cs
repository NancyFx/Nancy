namespace Nancy.Routing.Constraints
{
    /// <summary>
    /// Constraint for <see cref="int"/> route segments with value within a specified range.
    /// </summary>
    public class RangeRouteSegmentConstraint : ParameterizedRouteSegmentConstraintBase<int>
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>
        /// The constraint's name.
        /// </value>
        public override string Name
        {
            get { return "range"; }
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
        protected override bool TryMatch(string segment, string[] parameters, out int matchedValue)
        {
            int minRange;
            int maxRange;
            int intValue;

            if (parameters.Length == 2)
            {
                if (!this.TryParseInt(parameters[0], out minRange) ||
                    !this.TryParseInt(parameters[1], out maxRange) ||
                    !this.TryParseInt(segment, out intValue))
                {
                    matchedValue = default(int);
                    return false;
                }
            }
            else
            {
                matchedValue = default(int);
                return false;
            }

            if (intValue < minRange || intValue > maxRange)
            {
                matchedValue = default(int);
                return false;
            }

            matchedValue = intValue;
            return true;
        }
    }
}