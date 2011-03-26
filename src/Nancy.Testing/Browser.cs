namespace Nancy.Testing
{
    using System;
    using System.IO;
    using System.Text;
    using Nancy.Bootstrapper;
    using IO;

    /// <summary>
    /// Provides the capability of executing a request with Nancy, using a specific configuration provided by an <see cref="INancyBootstrapper"/> instance.
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
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse Delete(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("DELETE", path, browserContext);
        }

        /// <summary>
        /// Performs a GET requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse Get(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("GET", path, browserContext);
        }

        /// <summary>
        /// Performs a POST requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse Post(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("POST", path, browserContext);
        }

        /// <summary>
        /// Performs a PUT requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="BrowserResponse"/> instance of the executed request.</returns>
        public BrowserResponse Put(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("PUT", path, browserContext);
        }

        private BrowserResponse HandleRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var request =
                CreateRequest(method, path, browserContext);

            return new BrowserResponse(this.engine.HandleRequest(request));
        }

        private static void BuildRequestBody(IBrowserContextValues contextValues)
        {
            var useFormValues = !String.IsNullOrEmpty(contextValues.BodyString);
            var bodyContents = useFormValues ? contextValues.FormValues : contextValues.BodyString;
            var bodyBytes = bodyContents != null ? Encoding.UTF8.GetBytes(bodyContents) : new byte[] { };

            if (useFormValues && !contextValues.Headers.ContainsKey("Content-Type"))
            {
                contextValues.Headers["Content-Type"] = new[] { "application/x-www-form-urlencoded" };
            }

            contextValues.Body = new MemoryStream(bodyBytes);
        }

        private static Request CreateRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var context =
                new BrowserContext();

            browserContext.Invoke(context);

            var contextValues =
                (IBrowserContextValues)context;

            BuildRequestBody(contextValues);

            var requestStream =
                RequestStream.FromStream(contextValues.Body, 0, true);

            return new Request(method, path, contextValues.Headers, requestStream, contextValues.Protocol, contextValues.QueryString);
        }
    }
}