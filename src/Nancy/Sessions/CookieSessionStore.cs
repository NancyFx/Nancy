namespace Nancy.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class CookieSessionStore : ISessionStore
    {
        public static string CookieName = "_nc";
        
        public void Save(ISession session, Response response)
        {
            if (session == null || session.Count == 0) { return; }

            var sb = new StringBuilder();
            foreach (var kvp in session)
            {
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(kvp.Value.ToString()));
                sb.Append(";");
            }
            response.AddCookie(CookieName, sb.ToString());
        }
        public ISession Load(IRequest request)
        {
            var dictionary = new Dictionary<string, object>();
            if (request.Cookie.ContainsKey(CookieName))
            {
                var parts = HttpUtility.UrlDecode(request.Cookie[CookieName]).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')))
                {
                    dictionary[HttpUtility.UrlDecode(part[0])] = HttpUtility.UrlDecode(part[1]);
                }              
            }            
            return new Session(dictionary);
        }
    }
}