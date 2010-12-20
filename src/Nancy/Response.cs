namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Cookies;


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
            this.Cookies = new List<INancyCookie>(2);
        }

        public string ContentType { get; set; }

        public Action<Stream> Contents { get; set; }        

        public IDictionary<string, string> Headers { get; set; }

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
