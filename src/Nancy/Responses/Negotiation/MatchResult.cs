namespace Nancy.Responses.Negotiation
{
    /// <summary>
    /// Represents whether a processor has matched/can handle processing the response.
    /// Values are of increasing priority.
    /// </summary>
    public enum MatchResult
    {
        /// <summary>
        /// No match, nothing to see here, move along
        /// </summary>
        NoMatch,

        /// <summary>
        /// Will accept anything
        /// </summary>
        DontCare,

        /// <summary>
        /// Matched, but in a non-specific way such as a wildcard match or fallback
        /// </summary>
        NonExactMatch,

        /// <summary>
        /// Exact specific match
        /// </summary>
        ExactMatch
    }
}