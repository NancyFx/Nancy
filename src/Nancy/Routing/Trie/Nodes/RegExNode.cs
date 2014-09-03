namespace Nancy.Routing.Trie.Nodes
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// A regular expression capture node e.g. (?&lt;foo>\d{2,4})
    /// </summary>
    public class RegExNode : TrieNode
    {
        private Regex expression;

        private string[] groupNames;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 1000; }
        }

        public RegExNode(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.BuildRegEx();
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var match = this.expression.Match(segment);

            if (!match.Success)
            {
                return SegmentMatch.NoMatch;
            }

            var result = new SegmentMatch(true);
            foreach (var groupName in this.groupNames)
            {
                var group = match.Groups[groupName];
                if (group.Success)
                {
                    result.CapturedParameters[groupName] = group.Value;
                }
            }

            return result;
        }

        private void BuildRegEx()
        {
            this.expression = new Regex(this.RouteDefinitionSegment, RegexOptions.Compiled);
            this.groupNames = this.expression.GetGroupNames();
        }
    }
}