namespace Nancy.Demo.MarkdownViewEngine
{
    using System;
    using System.Text.RegularExpressions;
    using CsQuery;
    using MarkdownSharp;

    [Serializable]
    public class BlogModel
    {
        private static readonly Regex SSVESubstitution = new Regex("^@[^$]*?$",
                                                                   RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                                   RegexOptions.Multiline);

        private readonly Markdown parser = new Markdown();

        public BlogModel(string markdown)
        {

            Title = GetTitle(markdown);
            Abstract = GetAbstract(markdown);
        }

        public string Title { get; private set; }

        public string Abstract { get; private set; }

        public string StrippedTitle
        {
            get
            {
                CQ data = Title;
                return data.Text();
            }
        }

        private string GetTitle(string content)
        {
            string ssveRemoved = SSVESubstitution.Replace(content, "").Trim();
            return
                parser.Transform(ssveRemoved.Substring(0,
                                                       ssveRemoved.IndexOf(Environment.NewLine, StringComparison.Ordinal)));
        }

        private string GetAbstract(string content)
        {
            string ssveRemoved = SSVESubstitution.Replace(content, "").Trim();
            return
                parser.Transform(
                    ssveRemoved.Substring(ssveRemoved.IndexOf(Environment.NewLine, StringComparison.Ordinal), 175));
        }
    }
}