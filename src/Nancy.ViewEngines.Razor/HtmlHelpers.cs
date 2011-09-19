namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using System.Web;

    public class HtmlHelpers
    {
        public readonly IViewEngine engine;
        public readonly IRenderContext renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHelpers"/> class.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="renderContext"></param>
        public HtmlHelpers(IViewEngine engine, IRenderContext renderContext)
        {
            this.engine = engine;
            this.renderContext = renderContext;
        }

        public IHtmlString Partial(string viewName)
        {
            return this.Partial(viewName, null);
        }

        public IHtmlString Partial(string viewName, dynamic model)
        {
            ViewLocationResult view = this.renderContext.LocateView(viewName, model);

            Action<Stream> action = this.engine.RenderView(view, model, this.renderContext);
            var mem = new MemoryStream();

            action.Invoke(mem);
            mem.Position = 0;

            var reader = new StreamReader(mem);

            return new NonEncodedHtmlString(reader.ReadToEnd());
        }

        public IHtmlString Raw(string text)
        {
            return new NonEncodedHtmlString(text);
        }
    }
}