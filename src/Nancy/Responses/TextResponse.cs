namespace Nancy.Responses
{
    using System.Collections.Generic;
    using System.Text;

    using Nancy.Cookies;

    /// <summary>
    /// Represents a text (text/plain) response
    /// </summary>
    public class TextResponse : Response
    {
        /// <summary>
        /// Creates a new instance of the TextResponse class
        /// </summary>
        /// <param name="contents">Text content - defaults to empty if null</param>
        /// <param name="contentType">Content Type - defaults to text/plain</param>
        /// <param name="encoding">String encoding - UTF8 if null</param>
        public TextResponse(string contents, string contentType = "text/plain", Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;

            if (contents != null)
            {
                this.Contents = stream =>
                                {
                                    var data = encoding.GetBytes(contents);
                                    stream.Write(data, 0, data.Length);
                                };
            }
        }

        /// <summary>
        /// Creates a new instance of the TextResponse class
        /// </summary>
        /// <param name="statusCode">Status code - defaults to OK</param>
        /// <param name="contents">Text content - defaults to empty if null</param>
        /// <param name="encoding">String encoding - UTF8 if null</param>
        /// <param name="headers">Headers if required</param>
        /// <param name="cookies">Cookies if required</param>
        public TextResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string contents = null, Encoding encoding = null, IDictionary<string, string> headers = null, IEnumerable<INancyCookie> cookies = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            this.ContentType = "text/plain";
            this.StatusCode = statusCode;

            if (contents != null)
            {
                this.Contents = stream =>
                {
                    var data = encoding.GetBytes(contents);
                    stream.Write(data, 0, data.Length);
                };
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