namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using System.Web;

    /// <summary>
    /// Helpers to generate html content.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class HtmlHelpers<TModel>
    {
        private readonly TModel model;

        public readonly RazorViewEngine engine;
        public readonly IRenderContext renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHelpers"/> class.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="renderContext"></param>
        public HtmlHelpers(RazorViewEngine engine, IRenderContext renderContext, TModel model)
        {
            this.engine = engine;
            this.renderContext = renderContext;
            this.model = model;
        }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        public IHtmlString Partial(string viewName)
        {
            return this.Partial(viewName, null);
        }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        public IHtmlString Partial(string viewName, dynamic model)
        {
            var view = this.renderContext.LocateView(viewName, model);

            var response = this.engine.RenderView(view, model, this.renderContext);
            Action<Stream> action = response.Contents;
            var mem = new MemoryStream();

            action.Invoke(mem);
            mem.Position = 0;

            var reader = new StreamReader(mem);

            return new NonEncodedHtmlString(reader.ReadToEnd());
        }

        /// <summary>
        /// Returns an html string composed of raw, non-encoded text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IHtmlString Raw(string text)
        {
            return new NonEncodedHtmlString(text);
        }

        /// <summary>
        /// Creates an anti-forgery token.
        /// </summary>
        /// <returns></returns>
        public IHtmlString AntiForgeryToken()
        {
            var tokenKeyValue = this.renderContext.GetCsrfToken();

            return new NonEncodedHtmlString(String.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", tokenKeyValue.Key, tokenKeyValue.Value));
        }
    }
}