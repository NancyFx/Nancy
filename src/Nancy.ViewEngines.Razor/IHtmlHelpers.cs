namespace Nancy.ViewEngines.Razor
{
    /// <summary>
    /// Defines the functionality of a html helper.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface IHtmlHelpers<TModel>
    {
        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        IHtmlString Partial(string viewName);

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        IHtmlString Partial(string viewName, dynamic model);

        /// <summary>
        /// Returns an html string composed of raw, non-encoded text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the raw text.</returns>
        IHtmlString Raw(string text);

        /// <summary>
        /// Creates an anti-forgery token.
        /// </summary>
        /// <returns>An <see cref="IHtmlString"/> representation of the anti forgery token.</returns>
        IHtmlString AntiForgeryToken();
    }
}