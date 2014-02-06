namespace Nancy.Routing.Constraints
{
    using Nancy.Routing.Trie;

    /// <summary>
    /// Defines the functionality to constrain route matching.
    /// </summary>
    public interface IRouteSegmentConstraint
    {
        /// <summary>
        /// Determines whether the given constraint should be matched.
        /// </summary>
        /// <param name="constraint">The route constraint.</param>
        /// <returns><c>true</c> if the constraint matches, <c>false</c> otherwise.</returns>
        bool Matches(string constraint);

        /// <summary>
        /// Matches the segment and parameter name against the constraint.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>A <see cref="SegmentMatch"/> containing information about the captured parameters.</returns>
        SegmentMatch GetMatch(string constraint, string segment, string parameterName);
    }
}