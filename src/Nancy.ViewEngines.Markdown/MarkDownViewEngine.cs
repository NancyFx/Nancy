namespace Nancy.ViewEngines.Markdown
{
    using ViewEngines;
    using System.Collections.Generic;
    using Responses;
    using System.IO;
    using MarkdownSharp;
    using SuperSimpleViewEngine;
    using System.Text.RegularExpressions;

    public class MarkDownViewEngine : IViewEngine
    {
        /// <summary>
        /// Viewengine for rendering Markdown
        /// </summary>
        private readonly SuperSimpleViewEngine engineWrapper;

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
                                                                                    string markDown =
                                                                                        viewLocationResult.Contents().ReadToEnd();

                                                                                    var parser = new Markdown();
                                                                                    return parser.Transform(markDown);
                                                                                });

            var serverHtml = ParagraphSubstitution.Replace(html, "$1");

            var renderHtml = this.engineWrapper.Render(serverHtml, model, new MarkdownViewEngineHost(new NancyViewEngineHost(renderContext), renderContext));

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(renderHtml);
                writer.Flush();
            };

            return response;
        }
    }
}

