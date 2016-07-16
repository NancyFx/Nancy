namespace Nancy
{
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using Nancy.Session;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class DefaultCookieSessionProvider : SessionProvider
    {
        ICookieConfiguration cookieConfiguration;
        private CookieBasedSessionsConfiguration currentConfiguration;

        public CookieBasedSessionsConfiguration CookieBasedSessionsConfiguration
        {
            get { return currentConfiguration; }
        }


        public string CookieName
        {
            get { return cookieConfiguration.CookieName; }
        }
        
        public override void Initialize()
        {
            currentConfiguration = (CookieBasedSessionsConfiguration)GetInstance<CookieBasedSessionsConfiguration>().FirstOrDefault();
            cookieConfiguration = (DefaultCookieConfiguration)GetInstance<DefaultCookieConfiguration>().FirstOrDefault();

            if(cookieConfiguration==null)
            {
                cookieConfiguration = (ICookieConfiguration)Activator.CreateInstance(GetType<ICookieConfiguration>().FirstOrDefault());
            }
            
            if (this.currentConfiguration == null)
            {
                this.currentConfiguration = new CookieBasedSessionsConfiguration
                {
                    Serializer = new DefaultObjectSerializer()
                };
            };            
        }
        
        public override ISession New(NancyContext context)
        {
            var request = context.Request;
            var dictionary = new Dictionary<string, object>();
            
            return new NancySession(dictionary);
        }

        public override ISession Load(NancyContext context)
        {
            var request = context.Request;
            var dictionary = new Dictionary<string, object>();

            var cookieName = this.cookieConfiguration.CookieName;            
            var hmacProvider = this.currentConfiguration.CryptographyConfiguration.HmacProvider;
            var encryptionProvider = this.currentConfiguration.CryptographyConfiguration.EncryptionProvider;

            if (request.Cookies.ContainsKey(cookieName))
            {
                var cookieData = HttpUtility.UrlDecode(request.Cookies[cookieName]);
                var hmacLength = Base64Helpers.GetBase64Length(hmacProvider.HmacLength);
                if (cookieData.Length < hmacLength)
                {
                    return new NancySession(dictionary);
                }

                var hmacString = cookieData.Substring(0, hmacLength);
                var encryptedCookie = cookieData.Substring(hmacLength);

                var hmacBytes = Convert.FromBase64String(hmacString);
                var newHmac = hmacProvider.GenerateHmac(encryptedCookie);
                var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, hmacProvider.HmacLength);

                var data = encryptionProvider.Decrypt(encryptedCookie);
                var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')).Where(part => part.Length == 2))
                {
                    var valueObject = this.currentConfiguration.Serializer.Deserialize(HttpUtility.UrlDecode(part[1]));

                    dictionary[HttpUtility.UrlDecode(part[0])] = valueObject;
                }

                if (!hmacValid)
                {
                    dictionary.Clear();
                }

                return new NancySession(dictionary);
            }

            return null;
        }

        public override void Save(ISession session, NancyContext context)
        {
            if (session == null || !session.HasChanged)
            {
                return;
            }

            var sb = new StringBuilder();
            foreach (var kvp in session)
            {
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");

                var objectString = this.currentConfiguration.Serializer.Serialize(kvp.Value);

                sb.Append(HttpUtility.UrlEncode(objectString));
                sb.Append(";");
            }
            
            var cryptographyConfiguration = this.currentConfiguration.CryptographyConfiguration;
            var encryptedData = cryptographyConfiguration.EncryptionProvider.Encrypt(sb.ToString());
            var hmacBytes = cryptographyConfiguration.HmacProvider.GenerateHmac(encryptedData);
            var cookieData = HttpUtility.UrlEncode(String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedData));

            var cookie = new NancyCookie(this.cookieConfiguration.CookieName, cookieData, true)
            {
                Domain = this.cookieConfiguration.Domain,
                Path = this.cookieConfiguration.Path
            };
            context.Response.WithCookie(cookie);
        }
    }
}