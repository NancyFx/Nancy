namespace Nancy.Responses.Negotiation
{
    /// <summary>
    /// Represents whether a processor has matched / can handle a requested response
    /// </summary>
    public class ProcessorMatch
    {
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