namespace Nancy
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Nancy.Cookies;
    using Nancy.Responses.Negotiation;


    /// <summary>
    /// Provides strongly-typed access to HTTP request headers.
    /// </summary>
    public class RequestHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        private readonly IDictionary<string, IEnumerable<string>> headers;
        private readonly ConcurrentDictionary<string, IEnumerable<Tuple<string, decimal>>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHeaders"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public RequestHeaders(IDictionary<string, IEnumerable<string>> headers)
        {
            this.headers = new Dictionary<string, IEnumerable<string>>(headers, StringComparer.OrdinalIgnoreCase);
            this.cache = new ConcurrentDictionary<string, IEnumerable<Tuple<string, decimal>>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Content-types that are acceptable.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<Tuple<string, decimal>> Accept
        {
            get { return this.GetWeightedValues("Accept"); }
            set { this.SetHeaderValues("Accept", value, GetWeightedValuesAsStrings); }
        }

        /// <summary>
        /// Character sets that are acceptable.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<Tuple<string, decimal>> AcceptCharset
        {
            get { return this.GetWeightedValues("Accept-Charset"); }
            set { this.SetHeaderValues("Accept-Charset", value, GetWeightedValuesAsStrings); }
        }

        /// <summary>
        /// Acceptable encodings.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> AcceptEncoding
        {
            get { return this.GetSplitValues("Accept-Encoding"); }
            set { this.SetHeaderValues("Accept-Encoding", value, x => x); }
        }

        /// <summary>
        /// Acceptable languages for response.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<Tuple<string, decimal>> AcceptLanguage
        {
            get { return this.GetWeightedValues("Accept-Language"); }
            set { this.SetHeaderValues("Accept-Language", value, GetWeightedValuesAsStrings); }
        }

        /// <summary>
        /// Authorization header value for request.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Authorization
        {
            get { return this.GetValue("Authorization", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("Authorization", value, x => new[] { x }); }
        }

        /// <summary>
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> CacheControl
        {
            get { return this.GetValue("Cache-Control"); }
            set { this.SetHeaderValues("Cache-Control", value, x => x); }
        }

        /// <summary>
        /// Contains name/value pairs of information stored for that URL.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains <see cref="INancyCookie"/> instances if they are available; otherwise it will be empty.</value>
        public IEnumerable<INancyCookie> Cookie
        {
            get { return this.GetValue("Cookie", GetNancyCookies, Enumerable.Empty<INancyCookie>()); }
        }

        /// <summary>
        /// What type of connection the user-agent would prefer.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Connection
        {
            get { return this.GetValue("Connection", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("Connection", value, x => new[] { x }); }
        }

        /// <summary>
        /// The length of the request body in octets (8-bit bytes).
        /// </summary>
        /// <value>The length of the contents if it is available; otherwise 0.</value>
        public long ContentLength
        {
            get { return this.GetValue("Content-Length", x => Convert.ToInt64(x.First()), 0); }
            set { this.SetHeaderValues("Content-Length", value, x => new[] { x.ToString(CultureInfo.InvariantCulture) }); }
        }

        /// <summary>
        /// The mime type of the body of the request (used with POST and PUT requests).
        /// </summary>
        /// <value>A <see cref="MediaRange"/> containing the header value if it is available; otherwise <see langword="null"/>.</value>
        public MediaRange ContentType
        {
            get { return this.GetValue("Content-Type", x => new MediaRange(x.First()), null); }
            set { this.SetHeaderValues("Content-Type", value, x => new[] { x.ToString() }); }
        }

        /// <summary>
        /// The date and time that the message was sent.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the message was sent. If not available then <see langword="null"/> will be returned.</value>
        public DateTime? Date
        {
            get { return this.GetValue("Date", x => ParseDateTime(x.First()), null); }
            set { this.SetHeaderValues("Date", value, x => new[] { GetDateAsString(value) }); }
        }

        /// <summary>
        /// The domain name of the server (for virtual hosting), mandatory since HTTP/1.1
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Host
        {
            get { return this.GetValue("Host", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("Host", value, x => new[] { x }); }
        }

        /// <summary>
        /// Only perform the action if the client supplied entity matches the same entity on the server. This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> IfMatch
        {
            get { return this.GetValue("If-Match"); }
            set { this.SetHeaderValues("If-Match", value, x => x); }
        }

        /// <summary>
        /// Allows a 304 Not Modified to be returned if content is unchanged
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the requested resource must have been changed since. If not available then <see langword="null"/> will be returned.</value>
        public DateTime? IfModifiedSince
        {
            get { return this.GetValue("If-Modified-Since", x => ParseDateTime(x.First()), null); }
            set { this.SetHeaderValues("If-Modified-Since", value, x => new[] { GetDateAsString(value) }); }
        }

        /// <summary>
        /// Allows a 304 Not Modified to be returned if content is unchanged
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> that contains the header values if they are available; otherwise it will be empty.</value>
        public IEnumerable<string> IfNoneMatch
        {
            get { return this.GetValue("If-None-Match"); }
            set { this.SetHeaderValues("If-None-Match", value, x => x); }
        }

        /// <summary>
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string IfRange
        {
            get { return this.GetValue("If-Range", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("If-Range", value, x => new[] { x }); }
        }

        /// <summary>
        /// Only send the response if the entity has not been modified since a specific time.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance that specifies when the requested resource may not have been changed since. If not available then <see langword="null"/> will be returned.</value>
        public DateTime? IfUnmodifiedSince
        {
            get { return this.GetValue("If-Unmodified-Since", x => ParseDateTime(x.First()), null); }
            set { this.SetHeaderValues("If-Unmodified-Since", value, x => new[] { GetDateAsString(value) }); }

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
            get { return this.GetValue("Max-Forwards", x => Convert.ToInt32(x.First()), 0); }
            set { this.SetHeaderValues("Max-Forwards", value, x => new[] { x.ToString(CultureInfo.InvariantCulture) }); }
        }

        /// <summary>
        /// This is the address of the previous web page from which a link to the currently requested page was followed.
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string Referrer
        {
            get { return this.GetValue("Referer", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("Referer", value, x => new[] { x }); }
        }

        /// <summary>
        /// The user agent string of the user agent
        /// </summary>
        /// <value>A <see cref="string"/> containing the header value if it is available; otherwise <see cref="string.Empty"/>.</value>
        public string UserAgent
        {
            get { return this.GetValue("User-Agent", x => x.First(), string.Empty); }
            set { this.SetHeaderValues("User-Agent", value, x => new[] { x }); }
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
            return this.GetEnumerator();
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

        private static string GetDateAsString(DateTime? value)
        {
            return !value.HasValue ? null : value.Value.ToString("R", CultureInfo.InvariantCulture);
        }

        private IEnumerable<string> GetSplitValues(string header)
        {
            var values = this.GetValue(header);

            return values
                .SelectMany(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim())
                .ToList();
        }

        private IEnumerable<Tuple<string, decimal>> GetWeightedValues(string headerName)
        {
            return this.cache.GetOrAdd(headerName, r =>
            {
                var values = this.GetValue(r);
                var result = new List<Tuple<string, decimal>>();

                foreach (var header in values)
                {
                    var buffer = string.Empty;
                    var name = string.Empty;
                    var quality = string.Empty;
                    var isReadingQuality = false;
                    var isInQuotedSection = false;

                    for (var index = 0; index < header.Length; index++)
                    {
                        var character = header[index];

                        if (character.Equals(' ') && (index != header.Length - 1) && !isInQuotedSection)
                        {
                            continue;
                        }

                        if (character.Equals('"'))
                        {
                            isInQuotedSection = !isInQuotedSection;
                        }

                        if (isInQuotedSection)
                        {
                            buffer += character;

                            if (index != header.Length - 1)
                            {
                                continue; ;
                            }
                        }

                        if (character.Equals(';') || character.Equals(',') || (index == header.Length - 1))
                        {
                            if (!(character.Equals(';') || character.Equals(',')))
                            {
                                buffer += character;
                            }

                            if (isReadingQuality)
                            {
                                quality = buffer;
                            }
                            else
                            {
                                if (name.Length > 0)
                                {
                                    name += ';';
                                }

                                name += buffer;
                            }

                            buffer = string.Empty;
                            isReadingQuality = false;
                            isInQuotedSection = false;
                        }

                        if (character.Equals(';'))
                        {
                            continue;
                        }

                        if ((character.Equals('q') || character.Equals('Q')) && (index != header.Length - 1))
                        {
                            if (header[index + 1].Equals('='))
                            {
                                isReadingQuality = true;
                                continue;
                            }
                        }

                        if (isReadingQuality && character.Equals('='))
                        {
                            continue;
                        }

                        if (character.Equals(',') || (index == header.Length - 1))
                        {
                            var actualQuality = 1m;
                            decimal temp;

                            if (decimal.TryParse(quality, NumberStyles.Number, CultureInfo.InvariantCulture, out temp))
                            {
                                actualQuality = temp;
                            }

                            result.Add(new Tuple<string, decimal>(name, actualQuality));

                            name = string.Empty;
                            quality = string.Empty;
                            buffer = string.Empty;
                            isReadingQuality = false;

                            continue;
                        }

                        buffer += character;
                    }
                }

                return result.OrderByDescending(x => x.Item2);
            });
        }

        private static IEnumerable<INancyCookie> GetNancyCookies(IEnumerable<string> cookies)
        {
            if (cookies == null)
            {
                yield break;
            }

            foreach (var cookie in cookies)
            {
                var cookieStrings = cookie.Split(';');
                foreach (var cookieString in cookieStrings)
                {
                    var equalPos = cookieString.IndexOf('=');
                    if (equalPos >= 0)
                    {
                        yield return new NancyCookie(cookieString.Substring(0, equalPos).TrimStart(), cookieString.Substring(equalPos+1).TrimEnd());
                    }
                }
            }
        }

        private IEnumerable<string> GetValue(string name)
        {
            return this.GetValue(name, x => x, new string[] {});
        }

        private T GetValue<T>(string name, Func<IEnumerable<string>, T> converter, T defaultValue)
        {
            IEnumerable<string> values;

            if (!this.headers.TryGetValue(name, out values))
            {
                return defaultValue;
            }

            return converter.Invoke(values);
        }

        private static IEnumerable<string> GetWeightedValuesAsStrings(IEnumerable<Tuple<string, decimal>> values)
        {
            return values.Select(x => string.Concat(x.Item1, ";q=", x.Item2.ToString(CultureInfo.InvariantCulture)));
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

        private void SetHeaderValues<T>(string header, T value, Func<T, IEnumerable<string>> valueTransformer)
        {
            this.InvalidateCacheEntry(header);

            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (this.headers.ContainsKey(header))
                {
                    this.headers.Remove(header);
                }
            }
            else
            {
                this.headers[header] = valueTransformer.Invoke(value);
            }
        }

        private void InvalidateCacheEntry(string header)
        {
            IEnumerable<Tuple<string, decimal>> values;
            this.cache.TryRemove(header, out values);
        }
    }
}
