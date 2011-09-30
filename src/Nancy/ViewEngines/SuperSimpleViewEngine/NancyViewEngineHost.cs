namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System;

    public class NancyViewEngineHost : IViewEngineHost
    {
        private IRenderContext renderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyViewEngineHost"/> class.
        /// </summary>
        /// <param name="renderContext">
        /// The render context.
        /// </param>
        public NancyViewEngineHost(IRenderContext renderContext)
        {
            this.renderContext = renderContext;
        }

        /// <summary>
        /// Html "safe" encode a string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Encoded string</returns>
        public string HtmlEncode(string input)
        {
            return this.renderContext.HtmlEncode(input);
        }

        /// <summary>
        /// Get the contenst of a template
        /// </summary>
        /// <param name="templateName">Name/location of the template</param>
        /// <param name="model">Model to use to locate the template via conventions</param>
        /// <returns>Contents of the template, or null if not found</returns>
        public string GetTemplate(string templateName, object model)
        {
            var viewLocationResult = this.renderContext.LocateView(templateName, model);

            if (viewLocationResult == null)
            {
                return "[ERR!]";
            }

            return viewLocationResult.Contents.Invoke().ReadToEnd();
        }

        /// <summary>
        /// Gets a uri string for a named route
        /// </summary>
        /// <param name="name">Named route name</param>
        /// <param name="parameters">Parameters to use to expand the uri string</param>
        /// <returns>Expanded uri string, or null if not found</returns>
        public string GetUriString(string name, params string[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Expands a path to include any base paths
        /// </summary>
        /// <param name="path">Path to expand</param>
        /// <returns>Expanded path</returns>
        public string ExpandPath(string path)
        {
            return this.renderContext.ParsePath(path);
        }

        /// <summary>
        /// Get the anti forgery token form element
        /// </summary>
        /// <returns>String containin the form element</returns>
        public string AntiForgeryToken()
        {
            var tokenKeyValue = this.renderContext.GetCsrfToken();

            return string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"", tokenKeyValue.Key, tokenKeyValue.Value);
        }
    }
}