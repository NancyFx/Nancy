namespace Nancy.ViewEngines.Razor
{
    using System.IO;
    using System.Web;

    public class HtmlHelpers
    {
        private readonly IViewEngine engine;
        private readonly IRenderContext renderContext;

        private readonly Stream s;

        public HtmlHelpers(IViewEngine engine, IRenderContext renderContext, Stream s)
        {
            this.engine = engine;
            this.renderContext = renderContext;
            this.s = s;
        }

        public void Partial(string viewName, dynamic model)
        {
            ViewLocationResult view = this.renderContext.LocateView(viewName, model);

            var content = view.Contents.Invoke().ReadToEnd();

            var writer = new StreamWriter(s);
            writer.Write(content);
            writer.Flush();
        }
    }

    public abstract class NancyRazorViewBase
    {
        public TextWriter Writer { get; set; }

        public string Code { get; set; }

        public string Path { get; set; }

        public dynamic Model { get; set; }

        public HtmlHelpers Html { get; set; }

        public abstract void Execute();

        // Writes the results of expressions like: "@foo.Bar"
        public virtual void Write(object value)
        {
            WriteLiteral(HttpUtility.HtmlEncode(value));
        }

        // Writes literals like markup: "<p>Foo</p>"
        public virtual void WriteLiteral(object value)
        {
            Writer.Write(value);
        }
    }
}