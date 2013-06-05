namespace Nancy.Routing.Trie.Nodes
{
  using System;
  using System.Text.RegularExpressions;

  /// <summary>
  /// A node for standard captures combined with a literal e.g. {id}.png
  /// e.g. /images/{id}.png - this node will be hit for /images/1337.png
  /// e.g. /google{verification}.html - this node will be hit for /google3df5a2ca6ec52a5b.html
  /// </summary>
  public class CaptureNodeWithLiteral : TrieNode
  {
    private string parameterName;
    private string preLiteral;
    private string postLiteral;

    public static readonly Regex MatchRegex = new Regex(@"^(.*)(\{[a-zA-Z]+\})(.*)$", RegexOptions.Compiled);

    /// <summary>
    /// Score for this node
    /// </summary>
    public override int Score
    {
      get { return 100; }
    }

    public CaptureNodeWithLiteral(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
      : base(parent, segment, nodeFactory)
    {
      this.ExtractParameterName();
    }

    /// <summary>
    /// Matches the segment for a requested route
    /// </summary>
    /// <param name="segment">Segment string</param>
    /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
    public override SegmentMatch Match(string segment)
    {
      var match = SegmentMatch.NoMatch;
      if (segment.StartsWith(this.preLiteral) && segment.EndsWith(this.postLiteral))
      {
        match = new SegmentMatch(true);
        match.CapturedParameters.Add(this.parameterName, segment.Substring(this.preLiteral.Length, segment.Length - this.postLiteral.Length));
      }
      return match;
    }

    /// <summary>
    /// Extracts the parameter name and the literals for the segment
    /// </summary>
    private void ExtractParameterName()
    {
      var matches = MatchRegex.Match(this.RouteDefinitionSegment);
      this.preLiteral = matches.Groups[1].Captures[0].Value;
      this.parameterName = matches.Groups[2].Captures[0].Value.Trim('{', '}');
      this.postLiteral = matches.Groups[3].Captures[0].Value;
    }
  }
}