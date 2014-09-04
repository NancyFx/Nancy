using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Session.Server
{
    public abstract class SessionStorageProvider
    {
        public abstract ISession GetSession(string key);
        public abstract void SaveSession(string key,ISession session);
    }
}
