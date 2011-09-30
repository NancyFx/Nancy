namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    /// <summary>
    /// Provides the view engine with utility functions for
    /// encoding, locating partial view templates etc.
    /// </summary>
    public interface IViewEngineHost
    {
        /// <summary>
        /// Html "safe" encode a string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Encoded string</returns>
        string HtmlEncode(string input);

        /// <summary>
        /// Get the contenst of a template
        /// </summary>
        /// <param name="templateName">Name/location of the template</param>
        /// <param name="model">Model to use to locate the template via conventions</param>
        /// <returns>Contents of the template, or null if not found</returns>
        string GetTemplate(string templateName, object model);

        /// <summary>
        /// Gets a uri string for a named route
        /// </summary>
        /// <param name="name">Named route name</param>
        /// <param name="parameters">Parameters to use to expand the uri string</param>
        /// <returns>Expanded uri string, or null if not found</returns>
        string GetUriString(string name, params string[] parameters);

        /// <summary>
        /// Expands a path to include any base paths
        /// </summary>
        /// <param name="path">Path to expand</param>
        /// <returns>Expanded path</returns>
        string ExpandPath(string path);

        /// <summary>
        /// Get the anti forgery token form element
        /// </summary>
        /// <returns>String containin the form element</returns>
        string AntiForgeryToken();
    }
}