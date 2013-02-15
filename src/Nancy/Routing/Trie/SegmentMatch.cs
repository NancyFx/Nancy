namespace Nancy.Routing.Trie
{
    using System.Collections.Generic;

    public class SegmentMatch
    {
        public bool Matches { get; private set; }

        public IDictionary<string, object> CapturedParameters { get; private set; }

        public SegmentMatch(bool matches)
        {
            this.Matches = matches;

            if (matches)
            {
                this.CapturedParameters = new Dictionary<string, object>();
            }
        }

        private static SegmentMatch noMatch = new SegmentMatch(false);
        public static SegmentMatch NoMatch { get { return noMatch; } }
    }
}