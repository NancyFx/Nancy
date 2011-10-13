namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using IO;
    using Nancy.Extensions;
    using Session;

    /// <summary>
    /// Encapsulates HTTP-request information to an Nancy application.
    /// </summary>
    public class Request
    {
        private readonly List<HttpFile> files = new List<HttpFile>();
        private dynamic form = new DynamicDictionary();

        private IDictionary<string, string> cookies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="method">The HTTP data transfer method used by the client.</param>
        /// <param name="path">The path of the requested resource, relative to the "Nancy root". This shold not not include the scheme, host name, or query portion of the URI.</param>
        /// <param name="scheme">The HTTP protocol that was used by the client.</param>
        public Request(string method, string path, string scheme) 
            : this(method, new Url { Path = path, Scheme = scheme })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="method">The HTTP data transfer method used by the client.</param>
        /// <param name="path">The path of the requested resource, relative to the "Nancy root". This shold not not include the scheme, host name, or query portion of the URI.</param>
        /// <param name="headers">The headers that was passed in by the client.</param>
        /// <param name="body">The <see cref="Stream"/> that represents the incoming HTTP body.</param>
        /// <param name="scheme">The HTTP scheme that was used by the client.</param>
        /// <param name="query">The querystring data that was sent by the client.</param>
        public Request(string method, string path, IDictionary<string, IEnumerable<string>> headers, RequestStream body, string scheme, string query = null, string ip = null)
            : this(method, new Url { Path=path, Scheme = scheme, Query = query ?? String.Empty}, body, headers, ip)
        {
        }

        public Request(string method, Url url, RequestStream body = null, IDictionary<string, IEnumerable<string>> headers = null, string ip = null)
        {
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentOutOfRangeException("method");
            }

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (String.IsNullOrEmpty(url.Path))
            {
                throw new ArgumentOutOfRangeException("url.Path");
            }

            if (url.Scheme == null)
            {
                throw new ArgumentNullException("url.Scheme");
            }
            
            if (String.IsNullOrEmpty(url.Scheme))
            {
                throw new ArgumentOutOfRangeException("url.Scheme");
            }

            this.UserHostAddress = ip;

            this.Url = url;

            this.Method = method;

            this.Query = url.Query.AsQueryDictionary();

            this.Body = body ?? RequestStream.FromStream(new MemoryStream());

            this.Headers = new RequestHeaders(headers ?? new Dictionary<string, IEnumerable<string>>());

            this.Session = new NullSessionProvider();

            this.ParseFormData();
            this.RewriteMethod();
        }

        /// <summary>
        /// Gets the IP address of the client
        /// </summary>
        public string UserHostAddress { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the url
        /// </summary>
        public Url Url { get; private set; } 

        /// <summary>
        /// Gets the request path, relative to the base path.
        /// Used for route matching etc.
        /// </summary>
        public string Path
        {
            get
            {
                return this.Url.Path;
            }
        }

        /// <summary>
        /// Gets the querystring data of the requested resource.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/>instance, containing the key/value pairs of querystring data.</value>
        public dynamic Query { get; set; }

        /// <summary>
        /// Gets a <see cref="RequestStream"/> that can be used to read the incoming HTTP body
        /// </summary>
        /// <value>A <see cref="RequestStream"/> object representing the incoming HTTP body.</value>
        public RequestStream Body { get; private set; }

        /// <summary>
        /// Gets the request cookies.
        /// </summary>
        public IDictionary<string, string> Cookies
        {
            get { return this.cookies ?? (this.cookies = this.GetCookieData()); }
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// Gets the cookie data from the request header if it exists
        /// </summary>
        /// <returns>Cookies dictionary</returns>
        private IDictionary<string, string> GetCookieData()
        {
            var cookieDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!this.Headers.Cookie.Any())
            {
                return cookieDictionary;
            }

            var cookies = this.Headers["cookie"].First().TrimEnd(';').Split(';');
			foreach (var parts in cookies.Select (c => c.Split (new[] { '=' }, 2)))
            {
                cookieDictionary[parts[0].Trim()] = parts[1];
            }

            return cookieDictionary;
        }

        /// <summary>
        /// Gets a collection of files sent by the client-
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance, containing an <see cref="HttpFile"/> instance for each uploaded file.</value>
        public IEnumerable<HttpFile> Files
        {
            get { return this.files; }
        }

        /// <summary>
        /// Gets the form data of the request.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/>instance, containing the key/value pairs of form data.</value>
        /// <remarks>Currently Nancy will only parse form data sent using the application/x-www-url-encoded mime-type.</remarks>
        public dynamic Form
        {
            get { return this.form; }
        }

        /// <summary>
        /// Gets the HTTP headers sent by the client.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> containing the name and values of the headers.</value>
        /// <remarks>The values are stored in an <see cref="IEnumerable{T}"/> of string to be compliant with multi-value headers.</remarks>
        public RequestHeaders Headers { get; private set; }

        private void ParseFormData()
        {
            if (string.IsNullOrEmpty(this.Headers.ContentType))
            {
                return;
            }

            var contentType = this.Headers["content-type"].First();
            var mimeType = contentType.Split(';').First();
            if (mimeType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                var reader = new StreamReader(this.Body);
                this.form = reader.ReadToEnd().AsQueryDictionary();
            }

            if (!mimeType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            var boundary = Regex.Match(contentType, @"boundary=(?<token>[^\n\; ]*)").Groups["token"].Value;
            var multipart = new HttpMultipart(this.Body, boundary);

            foreach (var httpMultipartBoundary in multipart.GetBoundaries())
            {
                if (string.IsNullOrEmpty(httpMultipartBoundary.Filename))
                {
                    var reader = new StreamReader(httpMultipartBoundary.Value);
                    this.form[httpMultipartBoundary.Name] = reader.ReadToEnd();
                }
                else
                {
                    this.files.Add(new HttpFile(
                                       httpMultipartBoundary.ContentType,
                                       httpMultipartBoundary.Filename,
                                       httpMultipartBoundary.Value
                                       ));
                }
            }
        }

        private void RewriteMethod()
        {
            if (!this.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!this.Form["_method"].HasValue)
            {
                return;
            }

            this.Method = this.Form["_method"];
        }
    }
}
