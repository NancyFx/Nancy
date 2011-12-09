namespace Nancy.Diagnostics
{
    using System.IO;
    using Security;
    using ViewEngines;
    using ViewEngines.SuperSimpleViewEngine;

    public class DiagnosticsViewRenderer
    {
        private static readonly IViewResolver ViewResolver = new DiagnosticsViewResolver();

        private static readonly IViewEngine Engine = new SuperSimpleViewEngineWrapper();

        public Response this[string name]
        {
            get { return RenderView(name, null); }
        }

        public Response this[string name, dynamic model]
        {
            get { return RenderView(name, model); }
        }

        private static Response RenderView(string name, dynamic model)
        {
            var fullName = string.Concat(name, ".sshtml");

            var stream = GetBodyStream(fullName);

            var location = GetViewLocationResult(fullName, stream);

            var cache = new DefaultViewCache();

            var context = 
                new NancyContext();

            context.Items.Add(CsrfToken.DEFAULT_CSRF_KEY, "DIAGNOSTICSTOKEN");

            var renderContext = 
                new DefaultRenderContext(ViewResolver, cache, new ViewLocationContext() { Context = context });

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
    }
}