using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Nancy.Helpers;

namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;
    using System.Linq.Expressions;

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

            var response = this.Engine.RenderView(view, modelForPartial, this.RenderContext);
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
        /// Allows you to perform a child request and insert the output somewhere into a parent view.
        /// </summary>
        /// <param name="route">the route you wish to </param>
        /// <param name="queryStringParams"></param>
        /// <returns></returns>
        public IHtmlString Get(string route, object queryStringParams)
        {
            return Get(route, GetDictionaryFromObjectProps(queryStringParams));
        }

        public IHtmlString Get(string route, IDictionary<string, object> queryStringParams)
        {
            var currentContext = this.RenderContext.Context;
            var headers = new Dictionary<string, IEnumerable<string>>();
            foreach (var key in currentContext.Request.Headers.Keys)
            {
                headers[key] = currentContext.Request.Headers[key];
            }

            var queryString = GetQueryString(queryStringParams);

            var nancyEngine = currentContext.NancyEngine;
            var req = new Request("GET", route, headers, null, currentContext.Request.Url.Scheme, queryString, currentContext.Request.UserHostAddress);
            var childContext = nancyEngine.HandleRequest(req);

            Action<Stream> action = childContext.Response.Contents;
            var mem = new MemoryStream();
            action.Invoke(mem);
            mem.Position = 0;
            var reader = new StreamReader(mem);

            return new NonEncodedHtmlString(reader.ReadToEnd());
        }


        private static Dictionary<string, object> GetDictionaryFromObjectProps(object parameters)
        {
            var publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            var dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo property in parameters.GetType().GetProperties(publicAttributes))
            {
                if (property.CanRead)
                    dictionary.Add(property.Name, property.GetValue(parameters, null));
            }

            return dictionary;
        }

        private static string GetQueryString(IDictionary<string, object> d)
        {
            if (d == null || d.Count == 0)
                return "";

            var paramList = new List<string>();
            foreach (var keyAndVal in d)
            {
                var val = keyAndVal.Value == null ? "" : keyAndVal.Value.ToString();
                paramList.Add(string.Format("{0}={1}", HttpUtility.UrlEncode(keyAndVal.Key), HttpUtility.UrlEncode(val)));
            }

            return string.Join("&", paramList.ToArray());
        }

    }
}