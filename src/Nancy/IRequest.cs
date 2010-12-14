namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;

    using Nancy.Routing;

    public interface IRequest
    {
        string Uri { get; }

        string Method { get; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        Stream Body { get; }

        dynamic Form { get; }
    }

    public class Request : IRequest
    {
        public Request(string method, string uri, IDictionary<string, IEnumerable<string>> headers, Stream body)
        {
            if (method == null)
                throw new ArgumentNullException("method", "The value of the method parameter cannot be null.");
            
            if (method.Length == 0)
                throw new ArgumentOutOfRangeException("method", method, "The value of the method parameter cannot be empty.");

            if (uri == null)
                throw new ArgumentNullException("uri", "The value of the uri parameter cannot be null.");

            if (uri.Length == 0)
                throw new ArgumentOutOfRangeException("uri", uri, "The value of the uri parameter cannot be empty.");

            if (headers == null)
                throw new ArgumentNullException("headers", "The value of the headers parameter cannot be null.");

            if (body == null)
                throw new ArgumentNullException("body", "The value of the body parameter cannot be null.");

            this.Body = body;
            this.Headers = headers;
            this.Method = method;
            this.Uri = uri;
        }

        public Request(string method, string uri)
            : this(method, uri, new Dictionary<string, IEnumerable<string>>(), new MemoryStream())
        {
        }

        public Stream Body { get; set; }

        public dynamic Form
        {
            get
            {
                var reader = new StreamReader(this.Body);
                var coll = HttpUtility.ParseQueryString(reader.ReadToEnd());

                var ret = new RouteParameters();
                foreach (var key in coll.AllKeys)
                {
                    ret[key] = coll[key];
                }

                return ret;
            }
        }

        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public string Method { get; private set; }

        public string Uri { get; private set; }
    }
}