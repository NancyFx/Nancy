namespace Nancy.ViewEngines.Markdown
{
    using System;
    using System.Text.RegularExpressions;

    using MarkdownSharp;

    public static class MarkdownViewengineRender
    {
        /// <summary>
        /// A regex for removing paragraph tags that the parser inserts on unknown content such as @Section['Content']
        /// </summary>
        /// <remarks>
        ///  &lt;p>		- matches the literal string "&lt;p>"
        ///  (		- creates a capture group, so that we can get the text back by backreferencing in our replacement string
        ///  @		- matches the literal string "@"
        ///  [^&lt;]*	- matches any character other than the "&lt;" character and does this any amount of times
        ///  )		- ends the capture group
        ///  &lt;/p>	- matches the literal string "&lt;/p>"
        /// </remarks>
        private static readonly Regex ParagraphSubstitution = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Renders stand alone / master page
        /// </summary>
        /// <param name="templateContent">Template content</param>
        /// <returns>HTML converted to markdown</returns>
        public static string RenderMasterPage(string templateContent)
        {
            var second =
               templateContent.Substring(
                   templateContent.IndexOf("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase),
                   templateContent.IndexOf("<body", StringComparison.OrdinalIgnoreCase));

            var third = templateContent.Substring(second.Length);

            var forth = templateContent.Substring(second.Length, third.IndexOf(">", StringComparison.Ordinal) + 1);

            var header = second + forth;

            var toConvert = templateContent.Substring(header.Length,
                                                      (templateContent.IndexOf("</body>", StringComparison.Ordinal) -
                                                       (templateContent.IndexOf(forth, StringComparison.Ordinal) + forth.Length)));

            var footer =
                templateContent.Substring(templateContent.IndexOf("</body>", StringComparison.OrdinalIgnoreCase));

            var parser = new Markdown();

            var html = parser.Transform(toConvert.Trim());

            var serverHtml = ParagraphSubstitution.Replace(html, "$1");

            //TODO: The "Replace" is simply for unit testing HTML/MD strings. Probably needs improving
            return string.Concat(header, serverHtml, footer).Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
        }
    }
}
