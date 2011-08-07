namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using Cryptography;
    using Nancy.Extensions;
    using Security;
    using Session;

    /// <summary>
    /// Default render context implementation.
    /// </summary>
    public class DefaultRenderContext : IRenderContext
    {
        private readonly IViewResolver viewResolver;
        private readonly IViewCache viewCache;
        private readonly CryptographyConfiguration cryptographyConfiguration;
        private readonly ISessionObjectFormatter formatter;
        private readonly ViewLocationContext viewLocationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContext"/> class.
        /// </summary>
        /// <param name="viewResolver"></param>
        /// <param name="viewCache"></param>
        /// <param name="cryptographyConfiguration"></param>
        /// <param name="formatter"></param>
        /// <param name="viewLocationContext"></param>
        public DefaultRenderContext(IViewResolver viewResolver, IViewCache viewCache, CryptographyConfiguration cryptographyConfiguration, ISessionObjectFormatter formatter, ViewLocationContext viewLocationContext)
        {
            this.viewResolver = viewResolver;
            this.viewCache = viewCache;
            this.cryptographyConfiguration = cryptographyConfiguration;
            this.formatter = formatter;
            this.viewLocationContext = viewLocationContext;
        }

        /// <summary>
        /// Parses a path and returns an absolute url path, taking into account
        /// base directory etc.
        /// </summary>
        /// <param name="input">Input url such as ~/styles/main.css</param>
        /// <returns>Parsed absolut url path</returns>
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
            return Helpers.HttpUtility.HtmlEncode(input);
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
        /// <param name="salt">Optional salt</param>
        /// <returns>A tuple containing the name (cookie name and form/querystring name) and value</returns>
        public KeyValuePair<string, string> GenerateCsrfToken(string salt = null)
        {
            if (this.cryptographyConfiguration == null || this.formatter == null)
            {
                throw new InvalidOperationException("Csrf tokens cannot be generated as a cryptography configurati and Formatter were not specified");
            }

            var token = new CsrfToken { Salt = salt, CreatedDate = DateTime.Now };
            token.CreateRandomBytes();
            token.CreateHmac(this.cryptographyConfiguration.HmacProvider);

            var serializedToken = this.formatter.Serialize(token);

            return new KeyValuePair<string, string>(CsrfToken.DEFAULT_CSRF_KEY, serializedToken);
        }
    }
}