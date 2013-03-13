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

        /// <summary>
        ///<p>		- matches the literal string "<p>"
        ///(		- creates a capture group, so that we can get the text back by backreferencing in our replacement string
        ///@		- matches the literal string "@"
        ///[^<]*	- matches any character other than the "<" character and does this any amount of times
        ///)		- ends the capture group
        ///</p>	- matches the literal string "</p>"
        /// </summary>
        private static readonly Regex ParagraphSubstitution = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownViewEngineHost"/> class.
        /// </summary>
        /// <param name="viewEngineHost">A decorator <see cref="IViewEngineHost"/></param>
        /// <param name="renderContext">The render context.</param>
        public MarkdownViewEngineHost(IViewEngineHost viewEngineHost, IRenderContext renderContext)
        {
            this.viewEngineHost = viewEngineHost;
            this.renderContext = renderContext;
            this.Context = this.renderContext.Context;
            this.parser = new MarkdownSharp.Markdown();
        }

        /// <summary>
        /// Context object of the host application.
        /// </summary>
        /// <value>An instance of the context object from the host.</value>
        public object Context { get; private set; }

        /// <summary>
        /// Html "safe" encode a string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Encoded string</returns>
        public string HtmlEncode(string input)
        {
            return this.viewEngineHost.HtmlEncode(input);
        }

        /// <summary>
        /// Get the contenst of a template
        /// </summary>
        /// <param name="templateName">Name/location of the template</param>
        /// <param name="model">Model to use to locate the template via conventions</param>
        /// <returns>Contents of the template, or null if not found</returns>
        public string GetTemplate(string templateName, object model)
        {
            var viewLocationResult = this.renderContext.LocateView(templateName, model);

            if (viewLocationResult == null)
            {
                return "[ERR!]";
            }

            var templateContent = viewLocationResult.Contents.Invoke().ReadToEnd();

            if (viewLocationResult.Name.ToLower() == "master")
            {
                return RenderMasterPage(templateContent);
            }

            return parser.Transform(templateContent);
        }

        /// <summary>
        /// Renders the master page
        /// </summary>
        /// <param name="templateContent">The content of the master page</param>
        /// <returns>HTML of master page</returns>
        private string RenderMasterPage(string templateContent)
        {
            var header =
                templateContent.Substring(
                    templateContent.IndexOf("<!DOCTYPE html>", StringComparison.OrdinalIgnoreCase),
                    templateContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6);

            var toConvert =
                templateContent.Substring(
                    templateContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase) + 6,
                    (templateContent.IndexOf("</body>", StringComparison.OrdinalIgnoreCase) - 7) -
                    (templateContent.IndexOf("<body>", StringComparison.OrdinalIgnoreCase)));

            var footer =
                templateContent.Substring(templateContent.IndexOf("</body>", StringComparison.OrdinalIgnoreCase));

            var html = parser.Transform(toConvert);

            var serverHtml = ParagraphSubstitution.Replace(html, "$1");

            return string.Concat(header, serverHtml, footer);
        }

        /// <summary>
        /// Gets a uri string for a named route
        /// </summary>
        /// <param name="name">Named route name</param>
        /// <param name="parameters">Parameters to use to expand the uri string</param>
        /// <returns>Expanded uri string, or null if not found</returns>
        public string GetUriString(string name, params string[] parameters)
        {
            return this.viewEngineHost.GetUriString(name, parameters);
        }

        /// <summary>
        /// Expands a path to include any base paths
        /// </summary>
        /// <param name="path">Path to expand</param>
        /// <returns>Expanded path</returns>
        public string ExpandPath(string path)
        {
            return this.viewEngineHost.ExpandPath(path);
        }

        /// <summary>
        /// Get the anti forgery token form element
        /// </summary>
        /// <returns>String containin the form element</returns>
        public string AntiForgeryToken()
        {
            return this.viewEngineHost.AntiForgeryToken();
        }
    }
}
