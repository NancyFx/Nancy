namespace Nancy.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nancy.Helpers;

    public class CookieSessionStore : ISessionStore
    {
        private static byte[] salt;        
        public static string CookieName = "_nc";
        public static string Passphrase;
        public static string Salt
        {            
            set { salt = Encoding.UTF8.GetBytes(value); }
        }

        private readonly IEncryption encryption;

        public CookieSessionStore() : this(new Encryption()){}
        public CookieSessionStore(IEncryption encryption)
        {
            this.encryption = encryption;
        }

        public void Save(ISession session, Response response)
        {
            if (session == null || !session.HasChanged) { return; }

            var sb = new StringBuilder();
            foreach (var kvp in session)
            {
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(kvp.Value.ToString()));
                sb.Append(";");
            }
            response.AddCookie(CookieName, encryption.Encrypt(sb.ToString(), Passphrase, salt));
        }

        public ISession Load(Request request)
        {
            var dictionary = new Dictionary<string, object>();
            if (request.Cookie.ContainsKey(CookieName))
            {
                var data = encryption.Decrypt(HttpUtility.UrlDecode(request.Cookie[CookieName]), Passphrase, salt);
                var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')))
                {
                    dictionary[HttpUtility.UrlDecode(part[0])] = HttpUtility.UrlDecode(part[1]);
                }              
            }            
            return new Session(dictionary);
        }
    }
}