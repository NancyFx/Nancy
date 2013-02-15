namespace Nancy.Routing.Trie.Nodes
{
    using System.Text.RegularExpressions;

    public class RegExNode : TrieNode
    {
        private Regex expression;

        private string[] groupNames;

        public override int Score
        {
            get { return 1000; }
        }

        public RegExNode(TrieNode parent, string segment, TrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.BuildRegEx();
        }

        private void BuildRegEx()
        {
            this.expression = new Regex(this.RouteDefinitionSegment, RegexOptions.Compiled);
            this.groupNames = expression.GetGroupNames();
        }

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
    }
}