namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;

    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.Localization;
    using Nancy.Security;

    /// <summary>
    /// Default render context implementation.
    /// </summary>
    public class DefaultRenderContext : IRenderContext
    {
        private readonly IViewResolver viewResolver;
        private readonly IViewCache viewCache;

        private readonly ITextResource textResource;

        private readonly ViewLocationContext viewLocationContext;

        private TextResourceFinder textResourceFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContext"/> class.
        /// </summary>
        /// <param name="viewResolver"></param>
        /// <param name="viewCache"></param>
        /// <param name="textResource"></param>
        /// <param name="viewLocationContext"></param>
        public DefaultRenderContext(IViewResolver viewResolver, IViewCache viewCache, ITextResource textResource, ViewLocationContext viewLocationContext)
        {
            this.viewResolver = viewResolver;
            this.viewCache = viewCache;
            this.textResource = textResource;
            this.viewLocationContext = viewLocationContext;
            this.textResourceFinder = new TextResourceFinder(textResource, viewLocationContext.Context);
        }

        /// <summary>
        /// Gets the context of the current request.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context
        {
            get { return this.viewLocationContext.Context; }
        }

        /// <summary>
        /// Gets the view cache that is used by Nancy.
        /// </summary>
        /// <value>An <see cref="IViewCache"/> instance.</value>
        public IViewCache ViewCache
        {
            get { return this.viewCache; }
        }

        /// <summary>
        /// Gets the text resource for localisation
        /// </summary>
        public ITextResource TextResource
        {
            get { return this.textResource; }
        }

        /// <summary>
        /// Gets the text resource finder for localisation
        /// </summary>
        public dynamic TextResourceFinder
        {
            get { return this.textResourceFinder; }
        }

        /// <summary>
        /// Parses a path and returns an absolute url path, taking into account
        /// base directory etc.
        /// </summary>
        /// <param name="input">Input url such as ~/styles/main.css</param>
        /// <returns>Parsed absolute url path</returns>
        public string ParsePath(string input)
        {
            return this.viewLocationContext.Context.ToFullPath(input);
        }

        /// <summary>
        /// HTML encodes a string.
        /// </summary>
        /// <param name="input">The string that should be HTML encoded.</param>
        /// <returns>A HTML encoded <see cref="string"/>.</returns>
        public string HtmlEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// Locates a view that matches the provided <paramref name="viewName"/> and <paramref name="model"/>.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="model">The model that should be used when locating the view.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise, <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, dynamic model)
        {
            return this.viewResolver.GetViewLocation(viewName, model, this.viewLocationContext);
        }

        /// <summary>
        /// Generates a Csrf token.
        /// The token should be stored in a cookie and the form as a hidden field.
        /// In both cases the name should be the key of the returned key value pair.
        /// </summary>
        /// <returns>A tuple containing the name (cookie name and form/querystring name) and value</returns>
        public KeyValuePair<string, string> GetCsrfToken()
        {
            object tokenObject;
            if (!this.viewLocationContext.Context.Items.TryGetValue(CsrfToken.DEFAULT_CSRF_KEY, out tokenObject))
            {
                throw new InvalidOperationException("CSRF is not enabled on this request");
            }

            var tokenString = tokenObject as string;
            if (string.IsNullOrEmpty(tokenString))
            {
                throw new InvalidOperationException("CSRF object is invalid");
            }

            return new KeyValuePair<string, string>(CsrfToken.DEFAULT_CSRF_KEY, tokenString);
        }
    }
}