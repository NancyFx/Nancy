namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cookies;
    using Cryptography;
    using Helpers;

    /// <summary>
    /// Cookie based session storage
    /// </summary>
    public class CookieBasedSessionStore : ISessionStore
    {
        /// <summary>
        /// Encryption provider
        /// </summary>
        private readonly IEncryptionProvider encryptionProvider;

        /// <summary>
        /// Provider for generating hmacs
        /// </summary>
        private readonly IHmacProvider hmacProvider;

        /// <summary>
        /// Formatter for de/serializing the session objects
        /// </summary>
        private readonly IObjectSerializer serializer;

        /// <summary>
        /// Cookie name for storing session information
        /// </summary>
        private static string cookieName = "_nc";

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessions"/> class.
        /// </summary>
        /// <param name="cryptographyConfiguration">The cryptography configuration.</param>
        /// <param name="objectSerializer">Session object serializer to use</param>
        public CookieBasedSessionStore(CryptographyConfiguration cryptographyConfiguration, IObjectSerializer objectSerializer)
        {
            this.encryptionProvider = cryptographyConfiguration.EncryptionProvider;
            this.hmacProvider = cryptographyConfiguration.HmacProvider;
            this.serializer = objectSerializer;
        }

        /// <summary>
        /// Gets the cookie name that the session is stored in
        /// </summary>
        /// <returns>Cookie name</returns>
        public static string GetCookieName()
        {
            return cookieName;
        }

        /// <summary>
        /// Load the session from the session cookie
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="items">The items in the session</param>
        /// <returns>Whether the session was loaded</returns>
        public bool TryLoadSession(Request request, out IDictionary<string, object> items)
        {
            items = new Dictionary<string, object>();

            // TODO - configurable path?
            if (!request.Cookies.ContainsKey(cookieName))
            {
                return false;
            }

            var cookieData = HttpUtility.UrlDecode(request.Cookies[cookieName]);
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);
            var hmacString = cookieData.Substring(0, hmacLength);
            var encryptedCookie = cookieData.Substring(hmacLength);

            var hmacBytes = Convert.FromBase64String(hmacString);
            var newHmac = this.hmacProvider.GenerateHmac(encryptedCookie);
            var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, this.hmacProvider.HmacLength);

            var data = this.encryptionProvider.Decrypt(encryptedCookie);
            var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts.Select(part => part.Split('=')))
            {
                var valueObject = this.serializer.Deserialize(HttpUtility.UrlDecode(part[1]));

                items[HttpUtility.UrlDecode(part[0])] = valueObject;
            }

            if (!hmacValid)
            {
                items.Clear();
            }
            return true;
        }

        /// <summary>
        /// Serialize the session and store it in the session cookie
        /// </summary>
        /// <param name="context">The NancyContext</param>
        /// <param name="items">The items in the session</param>
        public void SaveSession(NancyContext context, IDictionary<string, object> items)
        {
            var sb = new StringBuilder();
            foreach (var kvp in items)
            {
                sb.Append(HttpUtility.UrlEncode(kvp.Key));
                sb.Append("=");

                string objectString = this.serializer.Serialize(kvp.Value);

                sb.Append(HttpUtility.UrlEncode(objectString));
                sb.Append(";");
            }

            // TODO - configurable path?
            var encryptedData = this.encryptionProvider.Encrypt(sb.ToString());
            var hmacBytes = this.hmacProvider.GenerateHmac(encryptedData);
            var cookieData = String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedData);

            var cookie = new NancyCookie(cookieName, cookieData, true);
            context.Response.AddCookie(cookie);
        }
    }
}
