namespace Nancy.ViewEngines.Markdown
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MarkdownSharp;

    using Nancy.ViewEngines.SuperSimpleViewEngine;

    public class MarkdownViewEngineHost : IViewEngineHost
    {
        private readonly IViewEngineHost viewEngineHost;
        private readonly IRenderContext renderContext;
        private readonly IEnumerable<string> validExtensions;
        private readonly Markdown parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownViewEngineHost"/> class.
        /// </summary>
        /// <param name="viewEngineHost">A decorator <see cref="IViewEngineHost"/></param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="viewExtensions">The allowed extensions</param>
        public MarkdownViewEngineHost(IViewEngineHost viewEngineHost, IRenderContext renderContext, IEnumerable<string> viewExtensions)
        {
            this.viewEngineHost = viewEngineHost;
            this.renderContext = renderContext;
            this.validExtensions = viewExtensions;
            this.Context = this.renderContext.Context;
            this.parser = new Markdown();
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
        /// Get the contents of a template
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

            string templateContent;
            using (var reader = viewLocationResult.Contents.Invoke())
                templateContent = reader.ReadToEnd();

            if (viewLocationResult.Name.ToLower() == "master" && validExtensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                return MarkdownViewengineRender.RenderMasterPage(templateContent);
            }

            if (!validExtensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                using (var reader = viewLocationResult.Contents.Invoke())
                    return reader.ReadToEnd();
            }

            return parser.Transform(templateContent);
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
        /// <returns>String containing the form element</returns>
        public string AntiForgeryToken()
        {
            return this.viewEngineHost.AntiForgeryToken();
        }
    }
}
