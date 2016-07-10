using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Session
{
    class CookieSessionProvider : ISessionProvider
    {
        public void Clear(NancyContext context)
        {
            throw new NotImplementedException();
        }

        public ISession New(NancyContext context)
        {
            throw new NotImplementedException();
        }

        public ISession Load(NancyContext context)
        {
            throw new NotImplementedException();
        }

        public void Save(ISession session, NancyContext context)
        {
            throw new NotImplementedException();
        }
    }
}
