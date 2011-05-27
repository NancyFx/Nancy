namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using System.Web;

    /// <summary>
    /// 
    /// </summary>
    public class NonEncodedHtmlString : IHtmlString
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEncodedHtmlString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public NonEncodedHtmlString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>An HTML-encoded string.</returns>
        public string ToHtmlString()
        {
            return value;
        }
    }

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