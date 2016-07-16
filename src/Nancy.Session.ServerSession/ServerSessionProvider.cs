using Nancy.Cookies;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;

namespace Nancy.Session
{
    public class ServerSessionProvider : SessionProvider
    {
        ICookieConfiguration cookieConfiguration;
        
        public const int SID_SIZE = 64;
        static ConcurrentDictionary<string, NancySession> sessions;
        
        static RandomNumberGenerator _rng;

        protected string cookieName
        {
            get { return cookieConfiguration.CookieName; }
        }

        static ServerSessionProvider()
        {
            _rng = RandomNumberGenerator.Create();
            sessions = new ConcurrentDictionary<string, NancySession>();
        }

        public override void Initialize()
        {
            cookieConfiguration = (ICookieConfiguration)Activator.CreateInstance(GetType<ICookieConfiguration>().FirstOrDefault());            
        }

        public override ISession Load(NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey(cookieName))
            {
                string sid = context.Request.Cookies[cookieName];

                if (sessions.ContainsKey(sid))
                    return sessions[sid];
                else
                    return null;
            }
            else
                return null;
        }

        public override void Clear(NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey(cookieName))
            {
                string sid = context.Request.Cookies[cookieName];

                if (sessions.ContainsKey(sid))
                {
                    sessions[sid].DeleteAll();
                    NancySession tmpSession = null;
                    sessions.TryRemove(sid, out tmpSession);
                }
            }
        }

        public override void Save(ISession session, NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey(cookieName))
            {
                string sid = context.Request.Cookies[cookieName];

                var cookie = new NancyCookie(cookieName, sid, true)
                {
                    Domain = this.cookieConfiguration.Domain,
                    Path = this.cookieConfiguration.Path
                };

                context.Response.WithCookie(cookie);
            }
        }

        public override ISession New(NancyContext context)
        {
            var session = new NancySession();
            //this forces session hasChanges=true so that Save() will be triggered
            session["touch"] = true;
            session.Delete("touch");

            string sid = null;
            do
            {
                byte[] bSid = new byte[SID_SIZE];
                _rng.GetBytes(bSid);
                sid = BitConverter.ToString(bSid).Replace("-", "");
            } while (sessions.ContainsKey(sid));

            context.Request.Cookies[cookieName] = sid;
            sessions[sid] = session;
            
            return session;
        }
    }
}
