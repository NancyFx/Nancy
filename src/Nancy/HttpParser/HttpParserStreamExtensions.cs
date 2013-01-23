namespace Nancy.HttpParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy.IO;

    /// <summary>
    /// Extensions for Http parsing.
    /// </summary>
    public static class HttpParserStreamExtensions
    {
        /// <summary>
        /// Read the http stream as a <see cref="Request"/>.
        /// </summary>
        /// <param name="stream">The http stream.</param>
        /// <param name="scheme">The url scheme.</param>
        /// <param name="ip">The ip address.</param>
        /// <returns>Returns the parse <see cref="Request"/>.</returns>
        public static Request ReadAsRequest(this Stream stream, string scheme = "http", string ip = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException("scheme");
            }

            var requestLine = RequestLineParser.Parse(stream);

            var url = new Url();

            if (requestLine.Uri.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                Uri uri;
                if (Uri.TryCreate(requestLine.Uri, UriKind.Absolute, out uri))
                {
                    url.Path = uri.AbsolutePath;
                    url.Query = uri.Query;
                    url.Fragment = uri.Fragment;
                    url.Scheme = uri.Scheme;
                }
            }
            else
            {
                var splitUri = requestLine.Uri.Split('?');
                url.Path = splitUri[0];
                url.Query = splitUri.Length == 2 ? splitUri[1] : string.Empty;
                url.Scheme = scheme;
            }

            var headers = HeaderParser.Parse(stream);
            IEnumerable<string> headerValues;
            if (headers.TryGetValue("Host", out headerValues))
            {
                url.HostName = headerValues.FirstOrDefault();
            }

            var nestedRequestStream = new RequestStream(new HttpMultipartSubStream(stream, stream.Position, stream.Length), stream.Length - stream.Position, true);

            var request = new Request(requestLine.Method, url, nestedRequestStream, headers, ip);

            return request;
        }
    }
}