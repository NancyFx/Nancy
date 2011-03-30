namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The value that is returned from a route that was invoked by a <see cref="Browser"/> instance.
    /// </summary>
    public class BrowserResponse
    {
        private DocumentWrapper responseBody;

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
        /// Gets the HTTP response body wrapped in a <see cref="DocumentWrapper"/> instance.
        /// </summary>
        /// <value>A <see cref="DocumentWrapper"/> instance that wrapps the HTTP response body.</value>
        public DocumentWrapper Body
        {
            get
            {
                if (this.responseBody == null)
                {
                    using (var contentsStream = new MemoryStream())
                    {
                        this.Context.Response.Contents.Invoke(contentsStream);
                        contentsStream.Position = 0;
                        this.responseBody = new DocumentWrapper(contentsStream);
                    }
                }

                return this.responseBody;
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