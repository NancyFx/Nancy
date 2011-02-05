namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy.Extensions;

    public class Request
    {
        private dynamic form;

        public Request(string method, string uri, string protocol)
            : this(method, uri, new Dictionary<string, IEnumerable<string>>(), new MemoryStream(), protocol)
        {
        }

        public Request(string method, string uri, IDictionary<string, IEnumerable<string>> headers, Stream body, string protocol, string query = "")
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

            if (protocol == null)
                throw new ArgumentNullException("protocol", "The value of the protocol parameter cannot be null.");

            if (protocol.Length == 0)
                throw new ArgumentOutOfRangeException("protocol", protocol, "The value of the protocol parameter cannot be empty.");

            this.Body = body;
            this.Headers = new Dictionary<string, IEnumerable<string>>(headers, StringComparer.OrdinalIgnoreCase);
            this.Method = method;
            this.Uri = uri;
            this.Protocol = protocol;
            this.Query = query.AsQueryDictionary();
        }

        public dynamic Query { get; set; }

        public Stream Body { get; set; }

        public dynamic Form
        {
            get { return this.form ?? (this.form = this.GetFormData()); }
        }

        private dynamic GetFormData()
        {
            if (this.Headers.Keys.Any(x => x.Equals("content-type", StringComparison.OrdinalIgnoreCase)))
            {
                var contentType = this.Headers["content-type"].First();
                if (contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    var reader = new StreamReader(this.Body);
                    return reader.ReadToEnd().AsQueryDictionary();
                }
            }
            return new DynamicDictionary();
        }

        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public string Method { get; private set; }

        public string Uri { get; private set; }

        public string Protocol { get; private set; }
    }
}