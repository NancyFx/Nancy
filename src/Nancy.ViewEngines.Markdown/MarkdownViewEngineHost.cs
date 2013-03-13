namespace Nancy.ViewEngines.Markdown
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using SuperSimpleViewEngine;
    using System;
    

    public class MarkdownViewEngineHost : IViewEngineHost
    {
        private readonly IViewEngineHost viewEngineHost;
        private readonly IRenderContext renderContext;
        private readonly MarkdownSharp.Markdown parser;
        private static readonly IEnumerable<string> validExtensions = new[] { "md", "markdown" };

        /// <summary>
        /// 
        ///<p>		- matches the literal string "<p>"
        ///(		- creates a capture group, so that we can get the text back by backreferencing in our replacement string
        ///@		- matches the literal string "@"
        ///[^<]*	- matches any character other than the "<" character and does this any amount of times
        ///)		- ends the capture group
        ///</p>	- matches the literal string "</p>"
        /// </summary>
        private static readonly Regex ParagraphSubstitution = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public MarkdownViewEngineHost(IViewEngineHost viewEngineHost, IRenderContext renderContext)
        {
            this.viewEngineHost = viewEngineHost;
            this.renderContext = renderContext;
            this.Context = this.renderContext.Context;
            this.parser = new MarkdownSharp.Markdown();
        }

        public object Context { get; private set; }

        public string HtmlEncode(string input)
        {
            return this.viewEngineHost.HtmlEncode(input);
        }

        public string GetTemplate(string templateName, object model)
        {
            var viewLocationResult = this.renderContext.LocateView(templateName, model);

            if (viewLocationResult == null)
            {
                return "[ERR!]";
            }

            var masterpartialContent = viewLocationResult.Contents.Invoke().ReadToEnd();

            if (!validExtensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                return masterpartialContent;
            }

            if (viewLocationResult.Name.ToLower() == "master")
            {
                string header = masterpartialContent.Substring(masterpartialContent.IndexOf("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase),
                                                                masterpartialContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6);

                string toConvert =
                    masterpartialContent.Substring(
                        masterpartialContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6,
                        (masterpartialContent.IndexOf("</body>", StringComparison.OrdinalIgnoreCase) - 7) -
                        (masterpartialContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase)));

                string footer = masterpartialContent.Substring(masterpartialContent.IndexOf("</body>", StringComparison.OrdinalIgnoreCase));

                string html = parser.Transform(toConvert);

                var serverHtml = ParagraphSubstitution.Replace(html, "$1");

                return string.Concat(header, serverHtml, footer);
            }
            
            return  parser.Transform(masterpartialContent);
        }


        public string GetUriString(string name, params string[] parameters)
        {
            return this.viewEngineHost.GetUriString(name, parameters);
        }

        public string ExpandPath(string path)
        {
            return this.viewEngineHost.ExpandPath(path);
        }

        public string AntiForgeryToken()
        {
            return this.viewEngineHost.AntiForgeryToken();
        }
    }
}
