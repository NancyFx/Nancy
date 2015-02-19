namespace Nancy.Routing.Trie.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Nancy.Routing.Constraints;

    /// <summary>
    /// A node multiple standard captures combined with a literal e.g. {id}.png.{thing}.{otherthing}
    /// </summary>
    public class CaptureNodeWithMultipleParameters : TrieNode
    {
        private static readonly Regex MatchRegex = new Regex(@"({?[^{}]*}?)", RegexOptions.Compiled);


        private readonly List<string> parameterNames = new List<string>();
        private readonly List<string> constraints = new List<string>();

        private string builtRegex = string.Empty;
        private IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints;

        private const string AssertStart = "^";
        private const string MatchParameter = "(.*)";
        private const string AssertEnd = "$";

        /// <summary>
        /// Captures parameters within segments that contain literals.
        ///     i.e:
        ///         /{file}.{name}
        ///         /{file}.html
        ///         /{major}.{minor}.{revision}B{build}
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="segment">The segment to match upon</param>
        /// <param name="nodeFactory">The factory</param>
        /// <param name="routeSegmentConstraints"></param>
        public CaptureNodeWithMultipleParameters(TrieNode parent, string segment, ITrieNodeFactory nodeFactory, IEnumerable<IRouteSegmentConstraint> routeSegmentConstraints)
            : base(parent, segment, nodeFactory)
        {
            this.routeSegmentConstraints = routeSegmentConstraints;
            this.ExtractParameterNames();
        }

        /// <summary>
        /// Determines whether this TrieNode should be used for the given segment.
        /// </summary>
        /// <param name="segment">The route segment</param>
        /// <returns>a boolean</returns>
        public static bool IsMatch(string segment)
        {
            return MatchRegex.Matches(segment).Cast<Group>().Count(g => g.Value != string.Empty) > 1;
        }

        private static bool IsParameterCapture(Capture match)
        {
            return match.Value.StartsWith("{") && match.Value.EndsWith("}");
        }

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 100; }
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            var match = SegmentMatch.NoMatch;
            var regex = new Regex(this.builtRegex, StaticConfiguration.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);

            if (regex.IsMatch(segment))
            {
                match = new SegmentMatch(true);
                var regexMatch = regex.Match(segment);
                for (var i = 1; i < regexMatch.Groups.Count; i++)
                {
                    match.CapturedParameters.Add(this.parameterNames[i - 1], regexMatch.Groups[i].Value);
                    if (!string.IsNullOrEmpty(this.constraints[i - 1]))
                    {
                        var routeSegmentConstraint = this.routeSegmentConstraints.FirstOrDefault(x => x.Matches(constraints[i-1]));
                        if (routeSegmentConstraint == null || !routeSegmentConstraint.GetMatch(this.constraints[i - 1], regexMatch.Groups[i].Value, this.parameterNames[i - 1]).Matches)
                        {
                            return SegmentMatch.NoMatch;
                        }
                    }
                }
            }
            return match;
        }

        /// <summary>
        /// Extracts the parameter name and the literals for the segment
        /// </summary>
        private void ExtractParameterNames()
        {
            var matches = MatchRegex.Matches(this.RouteDefinitionSegment);
            this.BuildRegex(AssertStart);
            foreach (Match match in matches)
            {
                if (IsParameterCapture(match))
                {
                    if (match.Value.Contains(":"))
                    {
                        var segmentSplit = match.Value.Trim('{', '}').Split(':');
                        this.parameterNames.Add(segmentSplit[0]);
                        this.constraints.Add(segmentSplit[1]);
                    }
                    else
                    {
                        this.parameterNames.Add(match.Value.Trim('{', '}'));
                        this.constraints.Add(string.Empty);
                    }
                    this.BuildRegex(MatchParameter);
                }
                else
                {
                    this.BuildRegex(Regex.Escape(match.Value));
                }
            }
            this.BuildRegex(AssertEnd);
        }

        private void BuildRegex(string regexSegment)
        {
            this.builtRegex += regexSegment;
        }
    }
}