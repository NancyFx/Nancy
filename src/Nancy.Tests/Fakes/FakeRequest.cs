namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;
    using System.IO;
    using IO;

    public class FakeRequest : Request
    {
        public FakeRequest(string method, string uri)
            : this(method, uri, new Dictionary<string, IEnumerable<string>>(), RequestStream.FromStream(new MemoryStream()), "http", string.Empty)
        {
        }

        public FakeRequest(string method, string uri, IDictionary<string, IEnumerable<string>> headers)
            : this(method, uri, headers, RequestStream.FromStream(new MemoryStream()), "http", string.Empty)
        {
        }

        public FakeRequest(string method, string uri, IDictionary<string, IEnumerable<string>> headers, RequestStream body, string protocol, string query)
            : base(method, uri, headers, body, protocol, query)
        {
        }
    }
}