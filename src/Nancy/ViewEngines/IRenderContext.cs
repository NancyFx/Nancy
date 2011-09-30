namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of the context that is passed into a view engine when the view is requested to be rendered.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Gets the view cache that is used by Nancy.
        /// </summary>
        /// <value>An <see cref="IViewCache"/> instance.</value>
        IViewCache ViewCache { get; }

        /// <summary>
        /// Parses a path and returns an absolute url path, taking into account
        /// base directory etc.
        /// </summary>
        /// <param name="input">Input url such as ~/styles/main.css</param>
        /// <returns>Parsed absolut url path</returns>
        string ParsePath(string input);
        
        /// <summary>
        /// HTML encodes a string.
        /// </summary>
        /// <param name="input">The string that should be HTML encoded.</param>
        /// <returns>A HTML encoded <see cref="string"/>.</returns>
        string HtmlEncode(string input);

        /// <summary>
        /// Locates a view that matches the provided <paramref name="viewName"/> and <paramref name="model"/>.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="model">The model that should be used when locating the view.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise, <see langword="null"/>.</returns>
        ViewLocationResult LocateView(string viewName, dynamic model);

        /// <summary>
        /// Gets the current Csrf token.
        /// The token should be stored in a cookie and the form as a hidden field.
        /// In both cases the name should be the key of the returned key value pair.
        /// </summary>
        /// <returns>A tuple containing the name (cookie name and form/querystring name) and value</returns>
        KeyValuePair<string, string> GetCsrfToken();
    }
}