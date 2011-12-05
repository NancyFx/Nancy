namespace Nancy.Diagnostics
{
    using System.IO;
    using Security;
    using ViewEngines;
    using ViewEngines.SuperSimpleViewEngine;

    public class DiagnosticsViewRenderer
    {
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
            var engine =
                new SuperSimpleViewEngineWrapper();

            name = string.Concat(name, ".sshtml");

            var view =
                new EmbeddedFileResponse(typeof(DiagnosticsViewRenderer).Assembly, "Nancy.Diagnostics.Resources", name);

            var stream =
                new MemoryStream();

            view.Contents.Invoke(stream);
            stream.Position = 0;

            var location = new ViewLocationResult(
                "Nancy/Diagnostics/Resources",
                name,
                "sshtml",
                () => new StreamReader(stream));

            var cache = new DefaultViewCache();

            var context = 
                new NancyContext();

            context.Items.Add(CsrfToken.DEFAULT_CSRF_KEY, "DIAGNOSTICSTOKEN");

            var renderContext = 
                new DefaultRenderContext(null, cache, new ViewLocationContext() { Context = context });

            return engine.RenderView(location, model, renderContext);
        }
    }
}