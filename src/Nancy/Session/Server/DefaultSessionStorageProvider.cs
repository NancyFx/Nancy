using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Session.Server
{
    public class DefaultSessionStorageProvider:SessionStorageProvider
    {
        Dictionary<string, ISession> _sessions;

        public DefaultSessionStorageProvider()
        {
            _sessions = new Dictionary<string, ISession>();
        }

        public override ISession GetSession(string key)
        {
            if (_sessions.ContainsKey(key))
            {
                return _sessions[key];
            }
            else
            {
                return null;
            }
        }
        
        public override void SaveSession(string key, ISession session)
        {
            if (!_sessions.ContainsKey(key))
            {
                _sessions[key] = session;
            }
        }
    }
}
