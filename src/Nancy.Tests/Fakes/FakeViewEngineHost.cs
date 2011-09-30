namespace Nancy.Tests.Fakes
{
    using System;

    using Nancy.ViewEngines.SuperSimpleViewEngine;

    public class FakeViewEngineHost : IViewEngineHost
    {
        public Func<string, object, string> GetTemplateCallback { get; set; }
        public Func<string, string> ExpandPathCallBack { get; set; }

        /// <summary>
        /// Html "safe" encode a string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Encoded string</returns>
        public string HtmlEncode(string input)
        {
            return input.Replace("&", "&amp;").
                Replace("<", "&lt;").
                Replace(">", "&gt;").
                Replace("\"", "&quot;");
        }

        /// <summary>
        /// Get the contenst of a template
        /// </summary>
        /// <param name="templateName">Name/location of the template</param>
        /// <param name="model">Model to use to locate the template via conventions</param>
        /// <returns>Contents of the template, or null if not found</returns>
        public string GetTemplate(string templateName, object model)
        {
            return this.GetTemplateCallback != null ? this.GetTemplateCallback.Invoke(templateName, model) : string.Empty;
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
            return this.ExpandPathCallBack != null ? this.ExpandPathCallBack.Invoke(path) : path;
        }

        /// <summary>
        /// Get the anti forgery token form element
        /// </summary>
        /// <returns>String containin the form element</returns>
        public string AntiForgeryToken()
        {
            return "CSRF";
        }
    }
}