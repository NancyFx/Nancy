namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;
    using System.IO;
    using IO;

    public class FakeRequest : Request
    {
        public FakeRequest(string method, string path)
            : this(method, path, new Dictionary<string, IEnumerable<string>>(), RequestStream.FromStream(new MemoryStream()), "http", string.Empty)
        {
        }

        public FakeRequest(string method, string path, IDictionary<string, IEnumerable<string>> headers)
            : this(method, path, headers, RequestStream.FromStream(new MemoryStream()), "http", string.Empty)
        {
        }

        public FakeRequest(string method, string path, string query)
            : this(method, path, new Dictionary<string, IEnumerable<string>>(), RequestStream.FromStream(new MemoryStream()), "http", query)
        {
        }

        public FakeRequest(string method, string path, IDictionary<string, IEnumerable<string>> headers, RequestStream body, string protocol, string query)
            : base(method, new Url { Path = path, Query = query, Scheme = protocol }, body, headers)
        {
        }
    }
}