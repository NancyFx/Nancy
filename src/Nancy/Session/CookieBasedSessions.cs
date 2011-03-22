namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Bootstrapper;
    using Cryptography;
    using Nancy.Cookies;
    using Nancy.Helpers;

    /// <summary>
    /// Cookie based session storage
    /// </summary>
    public class CookieBasedSessions : IFormatterSelector
    {
        /// <summary>
        /// Encryption pass phrase
        /// </summary>
        private readonly string passPhrase;

        /// <summary>
        /// Encryption salt
        /// </summary>
        private readonly byte[] salt;

        /// <summary>
        /// Encryption provider
        /// </summary>
        private readonly IEncryptionProvider encryptionProvider;

        /// <summary>
        /// Provider for generating hmacs
        /// </summary>
        private readonly IHmacProvider hmacProvider;

        /// <summary>
        /// Passphrase for generating hmacs
        /// </summary>
        private readonly string hmacPassphrase;

        /// <summary>
        /// Formatter for de/serializing the session objects
        /// </summary>
        private ISessionObjectFormatter formatter;

        /// <summary>
        /// Cookie name for storing session information
        /// </summary>
        private static string cookieName = "_nc";

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessions"/> class.
        /// </summary>
        /// <param name="encryptionProvider">The encryption provider.</param>
        /// <param name="hmacProvider">The hmac provider</param>
        /// <param name="passPhrase">The encryption pass phrase.</param>
        /// <param name="salt">The encryption salt.</param>
        /// <param name="hmacPassphrase">The hmac passphrase</param>
        /// <param name="sessionObjectFormatter">Session object formatter to use</param>
        public CookieBasedSessions(IEncryptionProvider encryptionProvider, IHmacProvider hmacProvider, string passPhrase, string salt, string hmacPassphrase, ISessionObjectFormatter sessionObjectFormatter)
        {
            this.encryptionProvider = encryptionProvider;
            this.hmacProvider = hmacProvider;
            this.passPhrase = passPhrase;
            this.salt = CreateSalt(salt);
            this.hmacPassphrase = hmacPassphrase;
            this.formatter = sessionObjectFormatter;
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
        /// Initialise and add cookie based session hooks to the application pipeine
        /// </summary>
        /// <param name="applicationPipelines">Application pipelines</param>
        /// <param name="encryptionProvider">Encryption provider for encrypting cookies</param>
        /// <param name="hmacProvider">The hmac provider</param>
        /// <param name="passPhrase">Encryption pass phrase</param>
        /// <param name="salt">Encryption salt</param>
        /// <param name="hmacPassphrase">The hmac passphrase</param>
        /// <returns>Formatter selector for choosing a non-default formatter</returns>
        public static IFormatterSelector Enable(IApplicationPipelines applicationPipelines, IEncryptionProvider encryptionProvider, IHmacProvider hmacProvider, string passPhrase, string salt, string hmacPassphrase)
        {
            var sessionStore = new CookieBasedSessions(encryptionProvider, hmacProvider, passPhrase, salt, hmacPassphrase, new DefaultSessionObjectFormatter());

            applicationPipelines.BeforeRequest.AddItemToEndOfPipeline(ctx => LoadSession(ctx, sessionStore));
            applicationPipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionStore));

            return sessionStore;
        }

        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeine with the default encryption provider.
        /// </summary>
        /// <param name="applicationPipelines">Application pipelines</param>
        /// <param name="passPhrase">Encryption pass phrase</param>
        /// <param name="salt">Encryption salt</param>
        /// <param name="hmacPassphrase">The hmac passphrase</param>
        /// <returns>Formatter selector for choosing a non-default formatter</returns>
        public static IFormatterSelector Enable(IApplicationPipelines applicationPipelines, string passPhrase, string salt, string hmacPassphrase)
        {
            return Enable(applicationPipelines, new DefaultEncryptionProvider(), new DefaultHmacProvider(), passPhrase, salt, hmacPassphrase);
        }

        /// <summary>
        /// Using the specified formatter
        /// </summary>
        /// <param name="newFormatter">Formatter to use</param>
        public void WithFormatter(ISessionObjectFormatter newFormatter)
        {
            this.formatter = newFormatter;
        }

        /// <summary>
        /// Save the session into the response
        /// </summary>
        /// <param name="session">Session to save</param>
        /// <param name="response">Response to save into</param>
        public void Save(ISession session, Response response)
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

                var objectString = this.formatter.Serialize(kvp.Value);

                sb.Append(HttpUtility.UrlEncode(objectString));
                sb.Append(";");
            }

            // TODO - configurable path?
            var encryptedData = this.encryptionProvider.Encrypt(sb.ToString(), this.passPhrase, this.salt);
            var hmacBytes = this.hmacProvider.GenerateHmac(encryptedData, this.hmacPassphrase);
            var cookieData = String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedData);

            var cookie = new NancyCookie(cookieName, cookieData, true);
            response.AddCookie(cookie);
        }

        /// <summary>
        /// Loads the session from the request
        /// </summary>
        /// <param name="request">Request to load from</param>
        /// <returns>ISession containing the load session values</returns>
        public ISession Load(Request request)
        {
            var dictionary = new Dictionary<string, object>();

            // TODO - configurable path?
            if (request.Cookies.ContainsKey(cookieName))
            {
                var cookieData = HttpUtility.UrlDecode(request.Cookies[cookieName]);
                var hmacLength = Base64Helpers.GetBase64Length(this.hmacProvider.HmacLength);
                var hmacString = cookieData.Substring(0, hmacLength);
                var encryptedCookie = cookieData.Substring(hmacLength);

                var hmacBytes = Convert.FromBase64String(hmacString);
                var newHmac = this.hmacProvider.GenerateHmac(encryptedCookie, this.hmacPassphrase);
                var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, this.hmacProvider.HmacLength);

                var data = this.encryptionProvider.Decrypt(encryptedCookie, this.passPhrase, this.salt);
                var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')))
                {
                    var valueObject = this.formatter.Deserialize(HttpUtility.UrlDecode(part[1]));

                    dictionary[HttpUtility.UrlDecode(part[0])] = valueObject;
                }

                if (!hmacValid)
                {
                    dictionary.Clear();
                }
            }

            return new Session(dictionary);
        }

        /// <summary>
        /// Saves the request session into the response
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        private static void SaveSession(NancyContext context, CookieBasedSessions sessionStore)
        {
            sessionStore.Save(context.Request.Session, context.Response);
        }

        /// <summary>
        /// Loads the request session
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        /// <returns>Always returns null</returns>
        private static Response LoadSession(NancyContext context, CookieBasedSessions sessionStore)
        {
            if (context.Request == null)
            {
                return null;
            }

            context.Request.Session = sessionStore.Load(context.Request);

            return null;
        }

        /// <summary>
        /// Create salt byes from salt stirng
        /// </summary>
        /// <param name="saltString">Salt string</param>
        /// <returns>Byte array for encryption salt</returns>
        private static byte[] CreateSalt(string saltString)
        {
            if (string.IsNullOrEmpty(saltString) || saltString.Length < 8)
            {
                throw new ArgumentException("Salt must be at least 8 characters long", "saltString");
            }

            return Encoding.UTF8.GetBytes(saltString);
        }
    }
}