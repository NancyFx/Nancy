namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;
    using Nancy.Session;
    using Nancy.Cryptography;

    public class FakeIdBasedSessionStore : AbstractIdBasedSessionStore
    {
        private Dictionary<string, IDictionary<string, object>> _Items = new Dictionary<string, IDictionary<string, object>>();

        public IDictionary<string, IDictionary<string, object>> Items { get { return _Items; } }

        public FakeIdBasedSessionStore(CryptographyConfiguration cryptographyConfiguration)
            : base(cryptographyConfiguration)
        {
        }

        protected override bool TryLoad(string id, out IDictionary<string, object> items)
        {
            return _Items.TryGetValue(id, out items);
        }

        protected override void Save(string id, IDictionary<string, object> items)
        {
            _Items[id] = items;
        }
    }
}
