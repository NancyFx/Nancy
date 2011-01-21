namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Nancy.Routing;
    using Sessions;

    public interface IRequest
    {
        string Uri { get; }

        string Method { get; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        Stream Body { get; }

        dynamic Form { get; }

        IDictionary<string, string> Cookie { get; }

        ISession Session { get; }

        bool SessionWasLoaded { get; }
		
		string Protocol { get; }
    }

    public class Request : IRequest
    {        
        private dynamic form;

        private IDictionary<string, string> cookie;

        private ISession session;

        private readonly ISessionStore sessionStore;
        
        public Request(string method, string uri, string protocol)
            : this(method, uri, new Dictionary<string, IEnumerable<string>>(), new MemoryStream(), new CookieSessionStore(), protocol)
        {
        }

        public Request(string method, string uri, IDictionary<string, IEnumerable<string>> headers, Stream body, ISessionStore sessionStore, string protocol)
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

            if (sessionStore == null)
                throw new ArgumentNullException("sessionStore", "The value of the sessionStore parameter cannot be null.");
            
           if (protocol == null)
                throw new ArgumentNullException("protocol", "The value of the protocol parameter cannot be null.");
				
           if (protocol.Length == 0)
                throw new ArgumentOutOfRangeException("protocol", protocol, "The value of the protocol parameter cannot be empty.");
				
            this.Body = body;
            this.Headers = new Dictionary<string, IEnumerable<string>>(headers, StringComparer.OrdinalIgnoreCase);
            this.Method = method;
            this.Uri = uri;			
            this.sessionStore = sessionStore;
			this.Protocol = protocol;
        }

        public Stream Body { get; set; }

        public dynamic Form
        {
            get { return this.form ?? (this.form = this.GetFormData()); }
        }

        public IDictionary<string, string> Cookie
        {
            get { return this.cookie ?? (this.cookie = this.GetCookieData()); }
        }

        public ISession Session
        {
            get { 
                if (session == null)
                {
                    SessionWasLoaded = true;
                    session = sessionStore.Load(this);
                }
                return session;
            }
        }

        public bool SessionWasLoaded { get; set; }


        private dynamic GetFormData()
        {
            var ret = new DynamicDictionary();

            if (this.Headers.Keys.Any(x => x.Equals("content-type", StringComparison.OrdinalIgnoreCase)))
            {
                var contentType = this.Headers["content-type"].First();
                if (contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    var reader = new StreamReader(this.Body);
                    var coll = HttpUtility.ParseQueryString(reader.ReadToEnd());

                    foreach (var key in coll.AllKeys)
                    {
                        if(key != null)
                        {
                            ret[key] = coll[key];
                        }
                    }        
                }
            }
            
            return ret;
        }

        private IDictionary<string, string> GetCookieData()
        {
            var ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (this.Headers.ContainsKey("cookie"))
            {                
                var cookies = this.Headers["cookie"].First().Split(';');
                foreach (var cookie in cookies)
                {
                    var parts = cookie.Split('=');
                    ret[parts[0].Trim()] = parts[1];
                }
            }
            return ret;
        }

        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public string Method { get; private set; }

        public string Uri { get; private set; }

        public string Protocol { get; private set; }
    }
}