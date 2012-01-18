namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cookies;

    /// <summary>
    /// Encapsulates HTTP-response information from an Nancy operation.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Null object representing no body    
        /// </summary>
        public static Action<Stream> NoBody = s => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            this.Contents = NoBody;
            this.ContentType = "text/html";
            this.Headers = new Dictionary<string, string>();
            this.StatusCode = HttpStatusCode.OK;
            this.Cookies = new List<INancyCookie>(2);
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        /// <remarks>The default value is <c>text/html</c>.</remarks>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the delegate that will render contents to the response stream.
        /// </summary>
        /// <value>An <see cref="Action{T}"/> delegate, containing the code that will render contents to the response stream.</value>
        /// <remarks>The host of Nancy will pass in the output stream after the response has been handed back to it by Nancy.</remarks>
        public Action<Stream> Contents { get; set; }

        /// <summary>
        /// Gets the collection of HTTP response headers that should be sent back to the client.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance, contaning the key/value pair of headers.</value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code that should be sent back to the client.
        /// </summary>
        /// <value>A <see cref="HttpStatusCode"/> value.</value>
        public HttpStatusCode StatusCode { get; set; }

        public IList<INancyCookie> Cookies { get; private set; }
        
        public Response AddCookie(string name, string value)
        {
            return AddCookie(name, value, null, null, null);
        }

        public Response AddCookie(string name, string value, DateTime? expires)
        {
            return AddCookie(name, value, expires, null, null);
        }
    
        public Response AddCookie(string name, string value, DateTime? expires, string domain, string path)
        {
            return AddCookie(new NancyCookie(name, value){ Expires = expires, Domain = domain, Path = path });
        }

        public Response AddCookie(INancyCookie nancyCookie)
        {
            Cookies.Add(nancyCookie);
            return this;
        }

        public static implicit operator Response(HttpStatusCode statusCode)
        {
            return new Response { StatusCode = statusCode };
        }

        public static implicit operator Response(int statusCode)
        {
            return new Response { StatusCode = (HttpStatusCode)statusCode };
        }

        public static implicit operator Response(string contents)
        {
            return new Response { Contents = GetStringContents(contents) };
        }

        public static implicit operator Response(Action<Stream> streamFactory)
        {
            return new Response { Contents = streamFactory };
        }

        protected static Action<Stream> GetStringContents(string contents)
        {
            return stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
        }
    }
}
