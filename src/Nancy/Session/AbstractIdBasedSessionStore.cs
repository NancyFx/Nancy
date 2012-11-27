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
    /// An abstract base class for building an <see cref="ISessionStore"/> implemtation that stores a session id in the user's cookies
    /// </summary>
    public abstract class AbstractIdBasedSessionStore : ISessionStore
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
        /// Cookie name to store session id
        /// </summary>
        private static string cookieName = "_nsid";

        public AbstractIdBasedSessionStore(CryptographyConfiguration cryptographyConfiguration)
        {
            this.encryptionProvider = cryptographyConfiguration.EncryptionProvider;
            this.hmacProvider = cryptographyConfiguration.HmacProvider;
        }

        public static string CookieName { get { return cookieName; } }

        public bool TryLoadSession(Request request, out IDictionary<string, object> items)
        {
            var cookieData = request.Cookies.ContainsKey(CookieName) ? HttpUtility.UrlDecode(request.Cookies[CookieName]) : String.Empty;
            var id = String.IsNullOrEmpty(cookieData) ? String.Empty : ExtractSessionId(cookieData);

            if (id == String.Empty)
            {
                items = new Dictionary<string, object>();
                return false;
            }

            var ok = TryLoad(id, out items);
            items = items ?? new Dictionary<string, object>();
            return ok;
        }

        public void SaveSession(NancyContext context, IDictionary<string, object> items)
        {
            var cookieData = context.Request.Cookies.ContainsKey(CookieName) ? HttpUtility.UrlDecode(context.Request.Cookies[CookieName]) : String.Empty;
            var id = String.IsNullOrEmpty(cookieData) ? String.Empty : ExtractSessionId(cookieData);

            if (id == String.Empty)
            {
                id = GenerateNewSessionId();

                var encryptedId = encryptionProvider.Encrypt(id);
                var hmacBytes = hmacProvider.GenerateHmac(encryptedId);
                cookieData = String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedId);
                context.Response.AddCookie(new NancyCookie(CookieName, cookieData, true));
            }
            Save(id, items);
        }

        /// <summary>
        /// Extract the session id from the encrypted cookie data
        /// </summary>
        /// <param name="cookieData">The value of the user's session cookie</param>
        /// <returns>The session id</returns>
        private string ExtractSessionId(string cookieData)
        {
            var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);
            var hmacString = cookieData.Substring(0, hmacLength);
            var encryptedId = cookieData.Substring(hmacLength);
            var hmacBytes = Convert.FromBase64String(hmacString);
            var newHmac = this.hmacProvider.GenerateHmac(encryptedId);
            var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, this.hmacProvider.HmacLength);

            return hmacValid ? this.encryptionProvider.Decrypt(encryptedId) : String.Empty;
        }

        /// <summary>
        /// Generates a new session id
        /// </summary>
        /// <returns>Session Id</returns>
        protected virtual string GenerateNewSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Attempt to load the session by id from the implemented store
        /// </summary>
        /// <param name="id">Session Id</param>
        /// <param name="items">Loaded session items</param>
        /// <returns>Successful load</returns>
        protected abstract bool TryLoad(string id, out IDictionary<string, object> items);

        /// <summary>
        /// Save the session items with the supplied id
        /// </summary>
        /// <param name="id">Session Id</param>
        /// <param name="items">Session items to save</param>
        protected abstract void Save(string id, IDictionary<string, object> items);
    }
}