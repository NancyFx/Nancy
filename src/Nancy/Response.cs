namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Extensions;

    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            this.Contents = GetStringContents(string.Empty);
            this.ContentType = "text/html";
            this.Headers = new Dictionary<string, string>();
            this.StatusCode = HttpStatusCode.OK;
        }

        public string ContentType { get; set; }

        public Action<Stream> Contents { get; set; }

        public String File { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }

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
                var writer = 
                    new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
        }

        public static Response WriteFile(string virtualPath)
        {
            return new Response
                   {
                       File = virtualPath
                   };            
        }
    }
}
