namespace Nancy.Hosting.Owin
{
    using System.Collections.Generic;
    using IO;

    public class NancyRequestParameters
    {
        public string Method { get; set; }

        public string Uri { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public RequestStream Body { get; set; }

        public string Protocol { get; set; }

        public string Query { get; set; }
    }
}