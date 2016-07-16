namespace Nancy.Demo.SessionProvider
{
    using Nancy.Session;
    using Nancy.Cookies;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This is a sample of the basic implementation for a session provider. Nancy will use auto discovery to load the session provider. If no custom implementation is found, the <cref="DefaultCookieProvider"></cref> will be used.
    /// </summary>
    public class DemoSessionProvider : SessionProvider
    {
        static Dictionary<string, NancySession> sessions;
        
        protected string cookieName
        {
            get { return "test"; }
        }

        static DemoSessionProvider()
        {
            sessions = new Dictionary<string, NancySession>();
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
                    sessions.Remove(sid);
                }
            }
        }

        public override void Save(ISession session, NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey(cookieName))
            {
                string sid = context.Request.Cookies[cookieName];

                var cookie = new NancyCookie(cookieName, sid, true) { };

                context.Response.WithCookie(cookie);
            }
        }

        public override ISession New(NancyContext context)
        {
            var session = new NancySession();
            //this forces session hasChanges=true so that Save() will be triggered
            session["touch"] = true;
            session.Delete("touch");
            Random rnd = new Random();
            string sid = rnd.NextDouble().ToString();
            context.Request.Cookies[cookieName] = sid;
            sessions[sid] = session;

            return session;
        }
    }
}
