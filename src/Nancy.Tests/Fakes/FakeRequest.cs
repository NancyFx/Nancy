namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;
    using System.IO;

    using Session;

    public class FakeRequest : Request
    {
        public FakeRequest(string method, string uri)
            : this(method, uri, new Dictionary<string, IEnumerable<string>>(), new MemoryStream(), "http", string.Empty)
        {
        }

        public FakeRequest(string method, string uri, IDictionary<string, IEnumerable<string>> headers, Stream body, string protocol, string query)
            : base(method, uri, headers, body, protocol, query)
        {
        }
    }
}