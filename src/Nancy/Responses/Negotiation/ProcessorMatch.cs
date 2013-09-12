namespace Nancy.Responses.Negotiation
{
    /// <summary>
    /// Represents whether a processor has matched / can handle a requested response
    /// </summary>
    public class ProcessorMatch
    {
        /// <summary>
        /// A <see cref="ProcessorMatch"/> with both <see cref="ModelResult"/> and <see cref="RequestedContentTypeResult"/> set to <see cref="MatchResult.NoMatch"/>.
        /// </summary>
        public static ProcessorMatch None = new ProcessorMatch
        {
            ModelResult = MatchResult.NoMatch,
            RequestedContentTypeResult = MatchResult.NoMatch
        };

        /// <summary>
        /// Gets or sets the match result based on the content type
        /// </summary>
        public MatchResult RequestedContentTypeResult { get; set; }

        /// <summary>
        /// Gets or sets the match result based on the model
        /// </summary>
        public MatchResult ModelResult { get; set; }
    }
}