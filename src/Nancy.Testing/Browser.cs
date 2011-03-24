namespace Nancy.Testing
{
    using System;
    using Bootstrapper;
    using IO;

    /// <summary>
    /// 
    /// </summary>
    public class Browser : IHideObjectMembers
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="Browser"/> class.
        /// </summary>
        /// <param name="bootstrapper">A <see cref="INancyBootstrapper"/> instance that determins the Nancy configuration that should be used by the browser.</param>
        public Browser(INancyBootstrapper bootstrapper)
        {
            this.bootstrapper = bootstrapper;
            this.bootstrapper.Initialise();
            this.engine = this.bootstrapper.GetEngine();
        }

        /// <summary>
        /// Performs a DELETE requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Delete(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("DELETE", path, browserContext);
        }

        /// <summary>
        /// Performs a GET requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Get(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("GET", path, browserContext);
        }

        /// <summary>
        /// Performs a POST requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Post(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("POST", path, browserContext);
        }

        /// <summary>
        /// Performs a PUT requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Put(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("PUT", path, browserContext);
        }

        private NancyContext HandleRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var request =
                CreateRequest(method, path, browserContext);

            return this.engine.HandleRequest(request);
        }

        private static Request CreateRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var context =
                new BrowserContext();

            browserContext.Invoke(context);

            var contextValues =
                (IBrowserContextValues)context;

            var requestStream =
                RequestStream.FromStream(contextValues.Body);

            return new Request(method, path, contextValues.Headers, requestStream, contextValues.Protocol);
        }
    }
}