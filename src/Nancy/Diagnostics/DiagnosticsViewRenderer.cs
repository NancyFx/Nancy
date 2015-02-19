namespace Nancy.Diagnostics
{
    using System.IO;
    using System.Linq;

    using Nancy.Localization;
    using Nancy.Responses;
    using Nancy.Security;
    using Nancy.ViewEngines;
    using Nancy.ViewEngines.SuperSimpleViewEngine;

    /// <summary>
    /// Renders diagnostics views from embedded resources.
    /// </summary>
    public class DiagnosticsViewRenderer
    {
        private readonly NancyContext context;
        private static readonly IViewResolver ViewResolver = new DiagnosticsViewResolver();

        private static readonly IViewEngine Engine = new SuperSimpleViewEngineWrapper(Enumerable.Empty<ISuperSimpleViewEngineMatcher>());

        /// <summary>
        /// Creates a new instance of the <see cref="DiagnosticsViewRenderer"/> class.
        /// </summary>
        /// <param name="context">A <see cref="NancyContext"/> instance.</param>
        public DiagnosticsViewRenderer(NancyContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Renders the diagnostics view with the provided <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the view to render.</param>
        /// <returns>A <see cref="Response"/> of the rendered view.</returns>
        public Response this[string name]
        {
            get { return RenderView(name, null, this.context); }
        }

        /// <summary>
        /// Renders the diagnostics view with the provided <paramref name="name"/> and <paramref name="model"/>.
        /// </summary>
        /// <param name="name">The name of the view to render.</param>
        /// <param name="model">The model that should be passed to the view engine during rendering.</param>
        /// <returns>A <see cref="Response"/> of the rendered view.</returns>
        public Response this[string name, dynamic model]
        {
            get { return RenderView(name, model, this.context); }
        }

        private static Response RenderView(string name, dynamic model, NancyContext context)
        {
            var fullName = string.Concat(name, ".sshtml");

            var stream = GetBodyStream(fullName);

            var location = GetViewLocationResult(fullName, stream);

            var cache = new DefaultViewCache();

            context.Items.Add(CsrfToken.DEFAULT_CSRF_KEY, "DIAGNOSTICSTOKEN");

            var renderContext =
                new DefaultRenderContext(ViewResolver, cache, new DummyTextResource(), new ViewLocationContext() { Context = context });

            return Engine.RenderView(location, model, renderContext);
        }

        private static Stream GetBodyStream(string name)
        {
            var view = new EmbeddedFileResponse(typeof(DiagnosticsViewRenderer).Assembly, "Nancy.Diagnostics.Views", name);

            var stream = new MemoryStream();

            view.Contents.Invoke(stream);
            stream.Position = 0;
            return stream;
        }

        private static ViewLocationResult GetViewLocationResult(string name, Stream bodyStream)
        {
            return new ViewLocationResult(
                "Nancy/Diagnostics/Views",
                name,
                "sshtml",
                () => new StreamReader(bodyStream));
        }

        internal class DiagnosticsViewResolver : IViewResolver
        {
            /// <summary>
            /// Locates a view based on the provided information.
            /// </summary>
            /// <param name="viewName">The name of the view to locate.</param>
            /// <param name="model">The model that will be used with the view.</param>
            /// <param name="viewLocationContext">A <see cref="ViewLocationContext"/> instance, containing information about the context for which the view is being located.</param>
            /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be found, otherwise <see langword="null"/>.</returns>
            public ViewLocationResult GetViewLocation(string viewName, dynamic model, ViewLocationContext viewLocationContext)
            {
                var fullName = string.Concat(viewName, ".sshtml");

                var stream = GetBodyStream(fullName);

                return GetViewLocationResult(fullName, stream);
            }
        }

        internal class DummyTextResource : ITextResource
        {
            public string this[string key, NancyContext context]
            {
                get
                {
                    return string.Empty;
                }
            }
        }
    }
}