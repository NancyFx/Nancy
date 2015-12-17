namespace Nancy.Testing
{
    using System;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Provides the capability of executing a request with Nancy, using a specific configuration provided by an <see cref="INancyBootstrapper"/> instance.
    ///  </summary>
    [Obsolete("Use Browser instead.", error: false)]
    public class LegacyBrowser : IHideObjectMembers
    {
        private readonly Browser browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Browser"/> class, with the
        /// provided <see cref="ConfigurableBootstrapper"/> configuration.
        /// </summary>
        /// <param name="action">The <see cref="ConfigurableBootstrapper"/> configuration that should be used by the bootstrapper.</param>
        /// <param name="defaults">The default <see cref="BrowserContext"/> that should be used in a all requests through this browser object.</param>
        [Obsolete("Use Browser instead.", error: false)]
        public LegacyBrowser(Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> action, Action<BrowserContext> defaults = null)
            : this(new ConfigurableBootstrapper(action), defaults)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegacyBrowser"/> class.
        /// </summary>
        /// <param name="bootstrapper">A <see cref="INancyBootstrapper"/> instance that determines the Nancy configuration that should be used by the browser.</param>
        /// <param name="defaults">The default <see cref="BrowserContext"/> that should be used in a all requests through this browser object.</param>
        [Obsolete("Use Browser instead.", error: false)]
        public LegacyBrowser(INancyBootstrapper bootstrapper, Action<BrowserContext> defaults = null)
        {
            this.browser = new Browser(bootstrapper, defaults);
        }

        /// <summary>
        /// Performs a DELETE request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Delete(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Delete(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a DELETE request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Delete(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Delete(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a GET request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Get(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Get(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a GET request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Get(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Get(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a HEAD request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Head(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Head(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a HEAD request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Head(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Head(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a OPTIONS request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Options(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Options(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a OPTIONS request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Options(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Options(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a PATCH request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Patch(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Patch(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a PATCH request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Patch(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Patch(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a POST request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Post(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Post(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a POST request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Post(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Post(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a PUT request against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Put(string path, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Put(path, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a PUT request against Nancy.
        /// </summary>
        /// <param name="url">The url that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        [Obsolete("Use Browser instead.", error: false)]
        public BrowserResponse Put(Url url, Action<BrowserContext> browserContext = null)
        {
            return this.browser.Put(url, browserContext).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Performs a request of the HTTP <paramref name="method"/>, on the given <paramref name="url"/>, using the
        /// provided <paramref name="browserContext"/> configuration.
        /// </summary>
        /// <param name="method">HTTP method to send the request as.</param>
        /// <param name="url">The URl of the request.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse HandleRequest(string method, Url url, Action<BrowserContext> browserContext)
        {
            return this.browser.HandleRequest(method, url, browserContext).GetAwaiter().GetResult();                      
        }

        /// <summary>
        /// Performs a request of the HTTP <paramref name="method"/>, on the given <paramref name="path"/>, using the
        /// provided <paramref name="browserContext"/> configuration.
        /// </summary>
        /// <param name="method">HTTP method to send the request as.</param>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse HandleRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            return this.browser.HandleRequest(method, path, browserContext).GetAwaiter().GetResult();
        }
    }
}
