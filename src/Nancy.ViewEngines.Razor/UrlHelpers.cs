namespace Nancy.ViewEngines.Razor
{
    /// <summary>
    /// Helpers for url related functions.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class UrlHelpers<TModel>
    {
        private readonly RazorViewEngine razorViewEngine;
        private readonly IRenderContext renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlHelpers&lt;TModel&gt;"/> class.
        /// </summary>
        /// <param name="razorViewEngine">The razor view engine.</param>
        /// <param name="renderContext">The render context.</param>
        public UrlHelpers(RazorViewEngine razorViewEngine, IRenderContext renderContext)
        {
            this.razorViewEngine = razorViewEngine;
            this.renderContext = renderContext;
        }

        /// <summary>
        /// Retrieves the absolute url of the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public string Content(string path)
        {
            return renderContext.ParsePath(path);
        }
    }
}