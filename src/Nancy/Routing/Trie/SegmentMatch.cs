namespace Nancy.Routing.Trie
{
    using System.Collections.Generic;

    /// <summary>
    /// A segment match result
    /// </summary>
    public class SegmentMatch
    {
        private static SegmentMatch noMatch = new SegmentMatch(false);

        /// <summary>
        /// Gets a value indicating whether the match was successful or not
        /// </summary>
        public bool Matches { get; private set; }

        /// <summary>
        /// Gets a <see cref="SegmentMatch"/> representing "no match"
        /// </summary>
        public static SegmentMatch NoMatch { get { return noMatch; } }

        /// <summary>
        /// Gets the captured parameters from the match, if the match was successful
        /// </summary>
        public IDictionary<string, object> CapturedParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMatch"/> class.
        /// </summary>
        /// <param name="matches">if set to <c>true</c> [matches].</param>
        public SegmentMatch(bool matches)
        {
            this.Matches = matches;

            if (matches)
            {
                this.CapturedParameters = new Dictionary<string, object>();
            }
        }

    }
}