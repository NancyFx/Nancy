namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    public class Response
    {
        public string ContentType { get; set; }

        public Action<Stream> Contents { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }

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
            return new Response
            {
                Contents = GetStringContents(contents), 
                ContentType = "text/html", 
                StatusCode = HttpStatusCode.OK
            };
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
    }
}