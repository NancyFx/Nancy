namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.RegularExpressions;

    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.IO;
    using Session;

    /// <summary>
    /// Encapsulates HTTP-request information to an Nancy application.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public class Request : IDisposable
    {
        private readonly List<HttpFile> files = new List<HttpFile>();
        private dynamic form = new DynamicDictionary();

        private IDictionary<string, string> cookies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="method">The HTTP data transfer method used by the client.</param>
        /// <param name="path">The path of the requested resource, relative to the "Nancy root". This should not include the scheme, host name, or query portion of the URI.</param>
        /// <param name="scheme">The HTTP protocol that was used by the client.</param>
        public Request(string method, string path, string scheme)
            : this(method, new Url { Path = path, Scheme = scheme })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="method">The HTTP data transfer method used by the client.</param>
        /// <param name="url">The <see cref="Url"/> of the requested resource</param>
        /// <param name="headers">The headers that was passed in by the client.</param>
        /// <param name="body">The <see cref="Stream"/> that represents the incoming HTTP body.</param>
        /// <param name="ip">The client's IP address</param>
        /// <param name="certificate">The client's certificate when present.</param>
        /// <param name="protocolVersion">The HTTP protocol version.</param>
        public Request(string method,
            Url url,
            RequestStream body = null,
            IDictionary<string, IEnumerable<string>> headers = null,
            string ip = null,
            byte[] certificate = null,
            string protocolVersion = null)
        {
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentOutOfRangeException("method");
            }

            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (url.Path == null)
            {
                throw new ArgumentNullException("url.Path");
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

            if (certificate != null && certificate.Length != 0)
            {
                this.ClientCertificate = new X509Certificate2(certificate);
            }

            this.ProtocolVersion = protocolVersion ?? string.Empty;

            if (String.IsNullOrEmpty(this.Url.Path))
            {
                this.Url.Path = "/";
            }

            this.ParseFormData();
            this.RewriteMethod();
        }

        /// <summary>
        /// Gets the certificate sent by the client.
        /// </summary>
        public X509Certificate ClientCertificate { get; private set; }

        /// <summary>
        /// Gets the HTTP protocol version.
        /// </summary>
        public string ProtocolVersion { get; private set; }

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
        /// Gets the query string data of the requested resource.
        /// </summary>
        /// <value>A <see cref="DynamicDictionary"/>instance, containing the key/value pairs of query string data.</value>
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

            var values = this.Headers["cookie"].First().TrimEnd(';').Split(';');
            foreach (var parts in values.Select(c => c.Split(new[] { '=' }, 2)))
            {
                var cookieName = parts[0].Trim();
                string cookieValue;

                if (parts.Length == 1)
                {
                    //Cookie attribute
                    cookieValue = string.Empty;
                }
                else
                {
                    cookieValue = HttpUtility.UrlDecode(parts[1]);
                }

                cookieDictionary[cookieName] = cookieValue;
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

        public void Dispose()
        {
            ((IDisposable)this.Body).Dispose();
        }

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
                this.Body.Position = 0;
            }

            if (!mimeType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var boundary = Regex.Match(contentType, @"boundary=""?(?<token>[^\n\;\"" ]*)").Groups["token"].Value;
            var multipart = new HttpMultipart(this.Body, boundary);

            var formValues =
                new NameValueCollection(StaticConfiguration.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

            foreach (var httpMultipartBoundary in multipart.GetBoundaries())
            {
                if (string.IsNullOrEmpty(httpMultipartBoundary.Filename))
                {
                    var reader =
                        new StreamReader(httpMultipartBoundary.Value);
                    formValues.Add(httpMultipartBoundary.Name, reader.ReadToEnd());

                }
                else
                {
                    this.files.Add(new HttpFile(httpMultipartBoundary));
                }
            }

            foreach (var key in formValues.AllKeys.Where(key => key != null))
            {
                this.form[key] = formValues[key];
            }

            this.Body.Position = 0;
        }

        private void RewriteMethod()
        {
            if (!this.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var overrides =
                new List<Tuple<string, string>>
                {
                    Tuple.Create("_method form input element", (string)this.Form["_method"]),
                    Tuple.Create("X-HTTP-Method-Override form input element", (string)this.Form["X-HTTP-Method-Override"]),
                    Tuple.Create("X-HTTP-Method-Override header", this.Headers["X-HTTP-Method-Override"].FirstOrDefault())
                };

            var providedOverride =
                overrides.Where(x => !string.IsNullOrEmpty(x.Item2));

            if (!providedOverride.Any())
            {
                return;
            }

            if (providedOverride.Count() > 1)
            {
                var overrideSources =
                    string.Join(", ", providedOverride);

                var errorMessage =
                    string.Format("More than one HTTP method override was provided. The provided values where: {0}", overrideSources);

                throw new InvalidOperationException(errorMessage);
            }

            this.Method = providedOverride.Single().Item2;
        }

        private string DebuggerDisplay
        {
            get { return string.Format("{0} {1} {2}", this.Method, this.Url, this.ProtocolVersion).Trim(); }
        }
    }
}
