namespace Nancy.Routing.Constraints
{
    using System;

    using Nancy.Routing.Trie;

    /// <summary>
    /// Convenience class for implementing a route segment constraint.
    /// </summary>
    /// <typeparam name="T">The type of parameter to capture.</typeparam>
    public abstract class RouteSegmentConstraintBase<T> : IRouteSegmentConstraint
    {
        /// <summary>
        /// Gets the name of the constraint.
        /// </summary>
        /// <value>The constraint's name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Determines whether the given constraint matches the name of this constraint.
        /// </summary>
        /// <param name="constraint">The route constraint.</param>
        /// <returns>
        ///   <c>true</c> if the constraint matches, <c>false</c> otherwise.
        /// </returns>
        public virtual bool Matches(string constraint)
        {
            return constraint.Equals(Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Matches the segment and parameter name against the constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>
        /// A <see cref="SegmentMatch" /> containing information about the captured parameters 
        /// stating whether there is a match or not.
        /// </returns>
        public SegmentMatch GetMatch(string constraint, string segment, string parameterName)
        {
            T value;
            if (this.TryMatch(constraint, segment, out value))
            {
                return CreateMatch(parameterName, value);
            }

            return SegmentMatch.NoMatch;
        }

        /// <summary>
        /// Tries to match the given segment against the constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="segment">The segment to match.</param>
        /// <param name="matchedValue">The matched value.</param>
        /// <returns><c>true</c> if the segment matches the constraint, <c>false</c> otherwise.</returns>
        protected abstract bool TryMatch(string constraint, string segment, out T matchedValue);

        private static SegmentMatch CreateMatch(string parameterName, object matchedValue)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters.Add(parameterName, matchedValue);
            return match;
        }
    }
}
