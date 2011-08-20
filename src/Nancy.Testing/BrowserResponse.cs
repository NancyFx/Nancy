namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The value that is returned from a route that was invoked by a <see cref="Browser"/> instance.
    /// </summary>
    public class BrowserResponse
    {
        private BrowserResponseBodyWrapper body;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserResponse"/> class.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> that <see cref="Browser"/> was invoked with.</param>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="context"/> parameter was <see langword="null"/>.</exception>
        public BrowserResponse(NancyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "The value of the context parameter cannot be null.");
            }

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
    }
}