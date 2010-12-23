namespace Nancy.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public class CookieSessionStore : ISessionStore
    {
        private static byte[] cookieSalt;        
        
        public static string CookieName = "_nc";
        public static string CookiePassphrase;
        public static string CookieSalt
        {         
            set { cookieSalt = Encoding.UTF8.GetBytes(value); }
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
            response.AddCookie(CookieName, Encrypt(sb.ToString()));
        }
        public ISession Load(IRequest request)
        {
            var dictionary = new Dictionary<string, object>();
            if (request.Cookie.ContainsKey(CookieName))
            {
                var parts = Decrypt(HttpUtility.UrlDecode(request.Cookie[CookieName])).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')))
                {
                    dictionary[HttpUtility.UrlDecode(part[0])] = HttpUtility.UrlDecode(part[1]);
                }              
            }            
            return new Session(dictionary);
        }

        private static string Encrypt(string data)
        {
            using (var secret = new Rfc2898DeriveBytes(CookiePassphrase, cookieSalt))
            using (var provider = new RijndaelManaged())
            using (var encryptor = provider.CreateEncryptor(secret.GetBytes(32), secret.GetBytes(16)))
            {
                byte[] input = Encoding.UTF8.GetBytes(data);
                byte[] output = encryptor.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output, 0, output.Length);
            }
        }

        private static string Decrypt(string data)
        {
            using (var secret = new Rfc2898DeriveBytes(CookiePassphrase, cookieSalt))
            using (var provider = new RijndaelManaged())
            using (var decryptor = provider.CreateDecryptor(secret.GetBytes(32), secret.GetBytes(16)))
            {
                var input = Convert.FromBase64String(data);
                var output = decryptor.TransformFinalBlock(input, 0, input.Length);
                return Encoding.UTF8.GetString(output, 0, output.Length);
            }
        }
    }
}