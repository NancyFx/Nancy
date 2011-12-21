namespace Nancy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Nancy.Cookies;

    /// <summary>
    /// Provides strongly-typed access to HTTP request headers.
    /// </summary>
    public class RequestHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        private readonly IDictionary<string, IEnumerable<string>> headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaders"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public RequestHeaders(IDictionary<string, IEnumerable<string>> headers)
        {
            this.headers = new Dictionary<string, IEnumerable<string>>(headers, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Content-types that are acceptable.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> Accept
        {
            get { return this.GetValue("Accept"); }
        }

        /// <summary>
        /// Character sets that are acceptable.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> AcceptCharset
        {
            get { return this.GetValue("Accept-Charset"); }
        }

        /// <summary>
        /// Acceptable encodings.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> AcceptEncoding
        {
            get { return this.GetValue("Accept-Encoding"); }
        }

        /// <summary>
        /// Acceptable languages for response.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> AcceptLanguage
        {
            get { return this.GetValue("Accept-Language"); }
        }

        /// <summary>
        /// Acceptable languages for response.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Authorization
        {
            get { return this.GetValue("Authorization", x => x.First()); }
        }

        /// <summary>
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> CacheControl
        {
            get { return this.GetValue("Cache-Control"); }
        }
        
        /// <summary>
        /// Contains name/value pairs of information stored for that URL.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains <see cref="INancyCookie"/> instances if they are available; otherwise it will be empty.</value>
        public IEnumerable<INancyCookie> Cookie
        {
            get { return this.GetValue("Cookie", GetNancyCookies); }
        }

        /// <summary>
        /// What type of connection the user-agent would prefer.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Connection
        {
            get { return this.GetValue("Connection", x => x.First()); }
        }

        /// <summary>
        /// The length of the request body in octets (8-bit bytes).
        /// </summary>
        /// <value>The lenght of the contents if it is available; otherwise 0.</value>
        public long ContentLength
        {
            get { return this.GetValue("Content-Length", x => Convert.ToInt64(x.First())); }
        }

        /// <summary>
        /// The mime type of the body of the request (used with POST and PUT requests).
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string ContentType
        {
            get { return this.GetValue("Content-Type", x => x.First()); }
        }

        /// <summary>
        /// The date and time that the message was sent.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the message was sent. If not available then <see cref="DateTime.MinValue"/> will be returned.</value>
        public DateTime? Date
        {
            get { return this.GetValue("Date", x => ParseDateTime(x.First())); }
        }

        /// <summary>
        /// The domain name of the server (for virtual hosting), mandatory since HTTP/1.1
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Host
        {
            get { return this.GetValue("Host", x => x.First()); }
        }

        /// <summary>
        /// Only perform the action if the client supplied entity matches the same entity on the server. This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> IfMatch
        {
            get { return this.GetValue("If-Match"); }
        }

        /// <summary>
        /// Allows a 304 Not Modified to be returned if content is unchanged
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the requested resource must have been changed since. If not available then <see cref="DateTime.MinValue"/> will be returned.</value>
        public DateTime? IfModifiedSince
        {
            get { return this.GetValue("If-Modified-Since", x => ParseDateTime(x.First())); }
        }

        /// <summary>
        /// Allows a 304 Not Modified to be returned if content is unchanged
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> IfNoneMatch
        {
            get { return this.GetValue("If-None-Match"); }
        }

        /// <summary>
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string IfRange
        {
            get { return this.GetValue("If-Range", x => x.First()); }
        }

        /// <summary>
        /// Only send the response if the entity has not been modified since a specific time.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the requested resource may not have been changed since. If not available then <see cref="DateTime.MinValue"/> will be returned.</value>
        public DateTime? IfUnmodifiedSince
        {
            get { return this.GetValue("If-Unmodified-Since", x => ParseDateTime(x.First())); }
        }

        /// <summary>
        /// Gets the names of the available request headers.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> containing the names of the headers.</value>
        public IEnumerable<string> Keys
        {
            get { return this.headers.Keys; }
        }

        /// <summary>
        /// Limit the number of times the message can be forwarded through proxies or gateways.
        /// </summary>
        /// <value>The number of the maximum allowed number of forwards if it is available; otherwise 0.</value>
        public int MaxForwards
        {
            get { return this.GetValue("Max-Forwards", x => Convert.ToInt32(x.First())); }
        }

        /// <summary>
        /// This is the address of the previous web page from which a link to the currently requested page was followed.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Referrer
        {
            get { return this.GetValue("Referer", x => x.First()); }
        }

        /// <summary>
        /// The user agent string of the user agent
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string UserAgent
        {
            get { return this.GetValue("User-Agent", x => x.First()); }
        }

        /// <summary>
        /// Gets all the header values.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains all the header values.</value>
        public IEnumerable<IEnumerable<string>> Values
        {
            get { return this.headers.Values; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return this.headers.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// Gets the values for the header identified by the <paramref name="name"/> parameter.
        /// </summary>
        /// <param name="name">The name of the header to return the values for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the values for the header. If the header is not defined then <see cref="Enumerable.Empty{TResult}"/> is returned.</returns>
        public IEnumerable<string> this[string name]
        {
            get
            {
                return (this.headers.ContainsKey(name)) ?
                    this.headers[name] :
                    Enumerable.Empty<string>();
            }
        }

        private static object GetDefaultValue(Type T)
        {
            if (IsGenericEnumerable(T))
            {
                var enumerableType = T.GetGenericArguments().First();
                var x = typeof(List<>).MakeGenericType(new[] { enumerableType });
                return Activator.CreateInstance(x);
            }

            if (T.Equals(typeof(DateTime)))
            {
                return null;
            }

            return T.Equals(typeof(string)) ?
                string.Empty :
                null;
        }

        private static IEnumerable<INancyCookie> GetNancyCookies(IEnumerable<string> cookies)
        {
            if(cookies == null)
            {
                return Enumerable.Empty<INancyCookie>();
            }

            return from cookie in cookies
                   let pair = cookie.Split('=')
                   select new NancyCookie(pair[0], pair[1]);
        }

        private IEnumerable<string> GetValue(string name)
        {
            return this.GetValue(name, x => x);
        }

        private T GetValue<T>(string name, Func<IEnumerable<string>, T> converter)
        {
            if (!this.headers.ContainsKey(name))
            {
                return (T)(GetDefaultValue(typeof(T)) ?? default(T));
            }

            return converter.Invoke(this.headers[name]);
        } 

        private static bool IsGenericEnumerable(Type T)
        {
            return !(T.Equals(typeof(string))) && T.IsGenericType && T.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>));
        }


        private static DateTime? ParseDateTime(string value)
        {
            DateTime result;
            // note CultureInfo.InvariantCulture is ignored
            if (DateTime.TryParseExact(value, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }
    }
}