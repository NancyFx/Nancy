namespace Nancy.Session
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Cryptography;

    /// <summary>
    /// An in-process implementation of <see cref="ISessionStgore"/> that stores the session id in user cookies
    /// </summary>
    public class InProcessSessionStore : AbstractIdBasedSessionStore
    {
        private static ConcurrentDictionary<string, IDictionary<string, object>> session = new ConcurrentDictionary<string, IDictionary<string, object>>();

        public InProcessSessionStore(CryptographyConfiguration cryptographyConfiguration) : base(cryptographyConfiguration) { }

        protected override bool TryLoad(string id, out IDictionary<string, object> items)
        {
            return session.TryGetValue(id, out items);
        }

        protected override void Save(string id, IDictionary<string, object> items)
        {
            session.AddOrUpdate(id, _ => items, (_, __) => items);
        }
    }
}
