namespace Nancy.Routing.Constraints
{
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Convenience class for implementing a route segment constraint that expects parameters.
    /// </summary>
    /// <typeparam name="T">The type of parameter to capture.</typeparam>
    public abstract class ParameterizedRouteSegmentConstraintBase<T> : RouteSegmentConstraintBase<T>
    {
        public override bool Matches(string constraint)
        {
            return constraint.Contains('(') && constraint.Contains(')') && base.Matches(constraint.Substring(0, constraint.IndexOf('(')));
        }

        protected override bool TryMatch(string constraint, string segment, out T matchedValue)
        {
            var parameters = constraint.Substring(constraint.IndexOf('(')).Trim('(', ')').Split(',');

            return TryMatch(segment, parameters, out matchedValue);
        }

        /// <summary>
        /// Tries to parse an integer using <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="string">The string value.</param>
        /// <param name="result">The resulting integer.</param>
        /// <returns><c>true</c> if the parsing was successful, <c>false</c> otherwise.</returns>
        protected bool TryParseInt(string @string, out int result)
        {
            return int.TryParse(@string, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// Tries to match the given segment and parameters against the constraint.
        /// </summary>
        /// <param name="segment">The segment to match.</param>
        /// <param name="parameters">The parameters to match.</param>
        /// <param name="matchedValue">The matched value.</param>
        /// <returns><c>true</c> if the segment and parameters matches the constraint, <c>false</c> otherwise.</returns>
        protected abstract bool TryMatch(string segment, string[] parameters, out T matchedValue);
    }
}