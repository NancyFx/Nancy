namespace Nancy.Responses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cookies;

    /// <summary>
    /// Represents a HTML (text/html) response
    /// </summary>
    public class HtmlResponse : Response
    {
        /// <summary>
        /// Creates a new instance of the HtmlResponse class
        /// </summary>
        /// <param name="statusCode">Status code - defaults to OK</param>
        /// <param name="contents">Response body delegate - defaults to empty if null</param>
        /// <param name="headers">Headers if required</param>
        /// <param name="cookies">Cookies if required</param>
        public HtmlResponse(HttpStatusCode statusCode = HttpStatusCode.OK, Action<Stream> contents = null, IDictionary<string, string> headers = null, IEnumerable<INancyCookie> cookies = null)
        {
            this.ContentType = "text/html";
            this.StatusCode = statusCode;

            if (contents != null)
            {
                this.Contents = contents;
            }

            if (headers != null)
            {
                this.Headers = headers;
            }

            if (cookies != null)
            {
                foreach (var nancyCookie in cookies)
                {
                    this.Cookies.Add(nancyCookie);
                }
            }
        }
    }
}