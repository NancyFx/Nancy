namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Nancy.Cookies;
    using Nancy.Helpers;

    /// <summary>
    /// Encapsulates HTTP-response information from an Nancy operation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Response: IDisposable
    {
        /// <summary>
        /// Null object representing no body
        /// </summary>
        public static Action<Stream> NoBody = s => { };

        private string contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            this.Contents = NoBody;
            this.ContentType = "text/html";
            this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.StatusCode = HttpStatusCode.OK;
            this.Cookies = new List<INancyCookie>(2);
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        /// <remarks>The default value is <c>text/html</c>.</remarks>
        public string ContentType
        {
            get { return Headers.ContainsKey("content-type") ? Headers["content-type"] : this.contentType; }
            set { this.contentType = value; }
        }

        /// <summary>
        /// Gets the delegate that will render contents to the response stream.
        /// </summary>
        /// <value>An <see cref="Action{T}"/> delegate, containing the code that will render contents to the response stream.</value>
        /// <remarks>The host of Nancy will pass in the output stream after the response has been handed back to it by Nancy.</remarks>
        public Action<Stream> Contents { get; set; }

        /// <summary>
        /// Gets the collection of HTTP response headers that should be sent back to the client.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance, containing the key/value pair of headers.</value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code that should be sent back to the client.
        /// </summary>
        /// <value>A <see cref="HttpStatusCode"/> value.</value>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code returned to the client.
        /// </summary>
        /// <value>The HTTP status code description.</value>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets the <see cref="INancyCookie"/> instances that are associated with the response.
        /// </summary>
        /// <value>A <see cref="IList{T}"/> instance, containing <see cref="INancyCookie"/> instances.</value>
        public IList<INancyCookie> Cookies { get; private set; }

        /// <summary>
        /// Executes at the end of the nancy execution pipeline and before control is passed back to the hosting.
        /// Can be used to pre-render/validate views while still inside the main pipeline/error handling.
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <returns>Task for completion/erroring</returns>
        public virtual Task PreExecute(NancyContext context)
        {
            return TaskHelpers.GetCompletedTask();
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        [Obsolete("This method has been replaced with Response.WithCookie and will be removed in a subsequent release.")]
        public Response AddCookie(string name, string value)
        {
            return AddCookie(name, value, null, null, null);
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        [Obsolete("This method has been replaced with Response.WithCookie and will be removed in a subsequent release.")]
        public Response AddCookie(string name, string value, DateTime? expires)
        {
            return AddCookie(name, value, expires, null, null);
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        /// <param name="domain">The domain of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        /// <returns>The <see cref="Response"/> instance.</returns>
        [Obsolete("This method has been replaced with Response.WithCookie and will be removed in a subsequent release.")]
        public Response AddCookie(string name, string value, DateTime? expires, string domain, string path)
        {
            return AddCookie(new NancyCookie(name, value){ Expires = expires, Domain = domain, Path = path });
        }

        /// <summary>
        /// Adds a <see cref="INancyCookie"/> to the response.
        /// </summary>
        /// <param name="nancyCookie">A <see cref="INancyCookie"/> instance.</param>
        /// <returns></returns>
        [Obsolete("This method has been replaced with Response.WithCookie and will be removed in a subsequent release.")]
        public Response AddCookie(INancyCookie nancyCookie)
        {
            Cookies.Add(nancyCookie);
            return this;
        }

        /// <summary>
        /// Implicitly cast an <see cref="HttpStatusCode"/> value to a <see cref="Response"/> instance, with the <see cref="StatusCode"/>
        /// set to the value of the <see cref="HttpStatusCode"/>.
        /// </summary>
        /// <param name="statusCode">The <see cref="HttpStatusCode"/> value that is being cast from.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public static implicit operator Response(HttpStatusCode statusCode)
        {
            return new Response { StatusCode = statusCode };
        }

        /// <summary>
        /// Implicitly cast an int value to a <see cref="Response"/> instance, with the <see cref="StatusCode"/>
        /// set to the value of the int.
        /// </summary>
        /// <param name="statusCode">The int value that is being cast from.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public static implicit operator Response(int statusCode)
        {
            return new Response { StatusCode = (HttpStatusCode)statusCode };
        }

        /// <summary>
        /// Implicitly cast an string instance to a <see cref="Response"/> instance, with the <see cref="Contents"/>
        /// set to the value of the string.
        /// </summary>
        /// <param name="contents">The string that is being cast from.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public static implicit operator Response(string contents)
        {
            return new Response { Contents = GetStringContents(contents) };
        }

        /// <summary>
        /// Implicitly cast an <see cref="Action{T}"/>, where T is a <see cref="Stream"/>, instance to
        /// a <see cref="Response"/> instance, with the <see cref="Contents"/> set to the value of the action.
        /// </summary>
        /// <param name="streamFactory">The <see cref="Action{T}"/> instance that is being cast from.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public static implicit operator Response(Action<Stream> streamFactory)
        {
            return new Response { Contents = streamFactory };
        }

        /// <summary>
        /// Implicitly cast a <see cref="DynamicDictionaryValue"/> instance to a <see cref="Response"/> instance,
        /// with the <see cref="Contents"/> set to the value of the <see cref="DynamicDictionaryValue"/>.
        /// </summary>
        /// <param name="value">The <see cref="DynamicDictionaryValue"/> instance that is being cast from.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public static implicit operator Response(DynamicDictionaryValue value)
        {
            return new Response { Contents = GetStringContents(value) };
        }

        /// <summary>
        /// Converts a string content value to a response action.
        /// </summary>
        /// <param name="contents">The string containing the content.</param>
        /// <returns>A response action that will write the content of the string to the response stream.</returns>
        protected static Action<Stream> GetStringContents(string contents)
        {
            return stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>This method can be overridden in sub-classes to dispose of response specific resources.</remarks>
        public virtual void Dispose()
        {
        }

        private string DebuggerDisplay
        {
            get { return string.Join(" ", new string[] { this.StatusCode.ToString(), this.ReasonPhrase, this.ContentType }.Where(x => !string.IsNullOrEmpty(x)).ToArray()); }
        }
    }
}
