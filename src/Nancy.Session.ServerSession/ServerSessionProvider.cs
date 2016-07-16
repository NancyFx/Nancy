using Nancy.Cookies;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;

namespace Nancy.Session
{
    /// <summary>
    /// Implements Server based session. Session values are stored in server's memory
    /// </summary>
    public class ServerSessionProvider : SessionProvider
    {
        private ICookieConfiguration cookieConfiguration;
        
        public const int SID_SIZE = 64;
        private static ConcurrentDictionary<string, NancySession> sessions;
        
        private static RandomNumberGenerator randomNumberGenerator;

        protected string cookieName
        {
            get { return this.cookieConfiguration.CookieName; }
        }

        static ServerSessionProvider()
        {
            randomNumberGenerator = RandomNumberGenerator.Create();
            sessions = new ConcurrentDictionary<string, NancySession>();
        }

        public override void Initialize()
        {
            this.cookieConfiguration = (ICookieConfiguration)Activator.CreateInstance(GetType<ICookieConfiguration>().FirstOrDefault());            
        }

        public override ISession Load(NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey(this.cookieName))
            {
                string sid = context.Request.Cookies[this.cookieName];

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
            if (context.Request.Cookies.ContainsKey(this.cookieName))
            {
                string sid = context.Request.Cookies[this.cookieName];

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
            if (context.Request.Cookies.ContainsKey(this.cookieName))
            {
                string sid = context.Request.Cookies[this.cookieName];

                var cookie = new NancyCookie(this.cookieName, sid, true)
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
                randomNumberGenerator.GetBytes(bSid);
                sid = BitConverter.ToString(bSid).Replace("-", "");
            } while (sessions.ContainsKey(sid));

            context.Request.Cookies[this.cookieName] = sid;
            sessions[sid] = session;
            
            return session;
        }
    }
}
