namespace Nancy.ViewEngines.Markdown
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using MarkdownSharp;

    using Nancy.Responses;
    using Nancy.ViewEngines.SuperSimpleViewEngine;

    /// <summary>
    /// Viewengine for rendering Markdown
    /// </summary>
    public class MarkDownViewEngine : IViewEngine
    {
        private readonly SuperSimpleViewEngine engineWrapper;

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
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return new[] { "md", "markdown" }; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkDownViewEngine"/> class.
        /// </summary>
        /// <param name="engineWrapper">The <see cref="SuperSimpleViewEngine"/> that should be used by the engine.</param>
        public MarkDownViewEngine(SuperSimpleViewEngine engineWrapper)
        {
            this.engineWrapper = engineWrapper;
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext">The render context.</param>
        /// <returns>A response.</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var response = new HtmlResponse();

            var html = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
                                                                                {
                                                                                    return ConvertMarkdown(viewLocationResult);
                                                                                });



            var renderHtml = this.engineWrapper.Render(html, model, new MarkdownViewEngineHost(new NancyViewEngineHost(renderContext), renderContext, this.Extensions));

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(renderHtml);
                writer.Flush();
            };

            return response;
        }

        /// <summary>
        /// Converts the markdown.
        /// </summary>
        /// <returns>
        /// HTML converted from markdown
        /// </returns>
        /// <param name='viewLocationResult'>
        /// View location result.
        /// </param>
        public string ConvertMarkdown(ViewLocationResult viewLocationResult)
        {
            string content;
            using (var reader = viewLocationResult.Contents.Invoke())
                content = reader.ReadToEnd();

            if (content.StartsWith("<!DOCTYPE html>"))
            {
                return MarkdownViewengineRender.RenderMasterPage(content);
            }

            var parser = new Markdown();
            var html = parser.Transform(content);
            return ParagraphSubstitution.Replace(html, "$1");
        }
    }
}

