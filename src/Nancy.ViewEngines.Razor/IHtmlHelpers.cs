namespace Nancy.ViewEngines.Razor
{
    using System.Web;

    /// <summary>
    /// Defines the functionality of a html helper
    /// </summary>
    public interface IHtmlHelpers
    {
        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        IHtmlString Partial(string viewName);

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        IHtmlString Partial(string viewName, dynamic model);

        /// <summary>
        /// Returns an html string composed of raw, non-encoded text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        IHtmlString Raw(string text);

        /// <summary>
        /// Creates an anti-forgery token.
        /// </summary>
        /// <returns></returns>
        IHtmlString AntiForgeryToken();
    }
}