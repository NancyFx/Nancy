namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;

    using Nancy.Cookies;

    /// <summary>
    /// The value that is returned from a route that was invoked by a <see cref="Browser"/> instance.
    /// </summary>
    public class BrowserResponse
    {
        private readonly Browser hostBrowser;

        private BrowserResponseBodyWrapper body;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserResponse"/> class.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> that <see cref="Browser"/> was invoked with.</param>
        /// <param name="hostBrowser">Host browser object</param>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="context"/> parameter was <see langword="null"/>.</exception>
        public BrowserResponse(NancyContext context, Browser hostBrowser)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "The value of the context parameter cannot be null.");
            }

            if (hostBrowser == null)
            {
                throw new ArgumentNullException("hostBrowser", "The value of the hostBrowser parameter cannot be null.");
            }

            this.hostBrowser = hostBrowser;

            this.Context = context;
        }

        /// <summary>
        /// Gets the HTTP response body as a <see cref="BrowserResponseBodyWrapper"/> instance.
        /// </summary>
        /// <value>A <see cref="BrowserResponseBodyWrapper"/> instance.</value>
        public BrowserResponseBodyWrapper Body
        {
            get
            {
                return this.body ?? (this.body = new BrowserResponseBodyWrapper(this.Context.Response));
            }
        }

        /// <summary>
        /// Gets the context that the <see cref="Browser"/> was invoked with.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context { get; private set; }

        /// <summary>
        /// Gets the headers of the response.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> instance, that contains the response headers.</value>
        public IDictionary<string, string> Headers
        {
            get { return this.Context.Response.Headers; }
        }
        
        /// <summary>
        /// Gets the HTTP status code of the response.
        /// </summary>
        /// <value>A <see cref="HttpStatusCode"/> enumerable value.</value>
        public HttpStatusCode StatusCode
        {
            get { return this.Context.Response.StatusCode; }
        }

        /// <summary>
        /// Gets the cookies from the response
        /// </summary>
        public IEnumerable<INancyCookie> Cookies
        {
            get { return this.Context.Response.Cookies; }
        }

        /// <summary>
        /// Continues with another request.
        /// </summary>
        public Browser Then
        {
            get
            {
                return this.hostBrowser;
            }
        }
    }
}