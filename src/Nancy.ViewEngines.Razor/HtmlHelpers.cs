namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using System.Security.Claims;

    using Nancy.Security;

    /// <summary>
    /// Helpers to generate html content.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class HtmlHelpers<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHelpers{T}"/> class.
        /// </summary>
        /// <param name="engine">The razor view engine instance that the helpers are being used by.</param>
        /// <param name="renderContext">The <see cref="IRenderContext"/> that the helper are being used by.</param>
        /// <param name="model">The model that is used by the page where the helpers are invoked.</param>
        public HtmlHelpers(RazorViewEngine engine, IRenderContext renderContext, TModel model)
        {
            this.Engine = engine;
            this.RenderContext = renderContext;
            this.Model = model;
        }

        /// <summary>
        /// The model that is being used by the current view.
        /// </summary>
        /// <value>An instance of the view model.</value>
        public TModel Model { get; set; }

        /// <summary>
        /// The engine that is currently rendering the view.
        /// </summary>
        /// <value>A <see cref="RazorViewEngine"/> instance.</value>
        public RazorViewEngine Engine { get; set; }

        /// <summary>
        /// The context of the current render operation.
        /// </summary>
        /// <value>An <see cref="IRenderContext"/> intance.</value>
        public IRenderContext RenderContext { get; set; }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        public IHtmlString Partial(string viewName)
        {
            return this.Partial(viewName, null);
        }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the partial view.</param>
        /// <param name="modelForPartial">The model that is passed to the partial.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        public IHtmlString Partial(string viewName, dynamic modelForPartial)
        {
            var view = this.RenderContext.LocateView(viewName, modelForPartial);

            var response = this.Engine.RenderView(view, modelForPartial, this.RenderContext, true);
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
        /// <returns>An <see cref="IHtmlString"/> representation of the raw text.</returns>
        public IHtmlString Raw(string text)
        {
            return new NonEncodedHtmlString(text);
        }

        /// <summary>
        /// Creates an anti-forgery token.
        /// </summary>
        /// <returns>An <see cref="IHtmlString"/> representation of the anti forgery token.</returns>
        public IHtmlString AntiForgeryToken()
        {
            var tokenKeyValue =
                this.RenderContext.GetCsrfToken();

            return new NonEncodedHtmlString(String.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", tokenKeyValue.Key, tokenKeyValue.Value));
        }

        /// <summary>
        /// Returns current culture name
        /// </summary>
        public string CurrentLocale
        {
            get { return this.RenderContext.Context.Culture.Name; }
        }

        /// <summary>
        /// Returns current authenticated user name
        /// </summary>
        public ClaimsPrincipal CurrentUser
        {
            get { return this.RenderContext.Context.CurrentUser; }
        }

        /// <summary>
        /// Determines if current user is authenticated
        /// </summary>
        public bool IsAuthenticated
        {
            get { return this.RenderContext.Context.CurrentUser != null; }
        }
    }
}