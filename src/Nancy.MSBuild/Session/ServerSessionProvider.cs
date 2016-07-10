using Nancy.Cookies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Session
{
    public class ServerSessionProvider : ISessionProvider
    {
        public const int SID_SIZE = 64;
        static ConcurrentDictionary<string, Session> sessions;
        
        static RNGCryptoServiceProvider _rng;
        static ServerSessionProvider()
        {
            _rng = new RNGCryptoServiceProvider();
            sessions = new ConcurrentDictionary<string, Session>();
        }

        public ISession Load(NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey("Nancy_SID"))
            {
                string sid = context.Request.Cookies["Nancy_SID"];

                if (sessions.ContainsKey(sid))
                    return sessions[sid];
                else
                    return null;
            }
            else
                return null;
        }

        public void Clear(NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey("Nancy_SID"))
            {
                string sid = context.Request.Cookies["Nancy_SID"];

                if (sessions.ContainsKey(sid))
                {
                    sessions[sid].DeleteAll();
                    Session tmpSession = null;
                    sessions.TryRemove(sid, out tmpSession);
                }
            }
        }

        public void Save(ISession session, NancyContext context)
        {
            if (context.Request.Cookies.ContainsKey("Nancy_SID"))
            {
                string sid = context.Request.Cookies["Nancy_SID"];
                var cookie = new NancyCookie("Nancy_SID", sid, true) { Expires = DateTime.Now.AddDays(1) };
                context.Response.WithCookie(cookie);
            }
        }

        public ISession New(NancyContext context)
        {
            var session = new Session();
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

            context.Request.Cookies["Nancy_SID"] = sid;
            sessions[sid] = session;
            
            return session;
        }
    }
}
