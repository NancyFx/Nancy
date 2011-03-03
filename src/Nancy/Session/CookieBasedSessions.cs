namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Bootstrapper;

    using Nancy.Cookies;
    using Nancy.Helpers;

    /// <summary>
    /// Cookie based session storage
    /// </summary>
    public class CookieBasedSessions
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
        /// Cookie name for storing session information
        /// </summary>
        private static string cookieName = "_nc";

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessions"/> class.
        /// </summary>
        /// <param name="encryptionProvider">The encryption provider.</param>
        /// <param name="passPhrase">The encryption pass phrase.</param>
        /// <param name="salt">The encryption salt.</param>
        public CookieBasedSessions(IEncryptionProvider encryptionProvider, string passPhrase, string salt)
        {
            this.encryptionProvider = encryptionProvider;
            this.passPhrase = passPhrase;
            this.salt = CreateSalt(salt);
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
        /// <param name="passPhrase">Encryption pass phrase</param>
        /// <param name="salt">Encryption salt</param>
        public static void Enable(IApplicationPipelines applicationPipelines, IEncryptionProvider encryptionProvider, string passPhrase, string salt)
        {
            var sessionStore = new CookieBasedSessions(encryptionProvider, passPhrase, salt);

            applicationPipelines.BeforeRequest.AddItemToEndOfPipeline(ctx => LoadSession(ctx, sessionStore));
            applicationPipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionStore));
        }

        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeine with the default encryption provider.
        /// </summary>
        /// <param name="applicationPipelines">Application pipelines</param>
        /// <param name="passPhrase">Encryption pass phrase</param>
        /// <param name="salt">Encryption salt</param>
        public static void Enable(IApplicationPipelines applicationPipelines, string passPhrase, string salt)
        {
            Enable(applicationPipelines, new DefaultEncryptionProvider(), passPhrase, salt);
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
                sb.Append(HttpUtility.UrlEncode(kvp.Value.ToString()));
                sb.Append(";");
            }

            // TODO - configurable path?
            var cookie = new NancyCookie(cookieName, this.encryptionProvider.Encrypt(sb.ToString(), this.passPhrase, this.salt), true);
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
                var data = this.encryptionProvider.Decrypt(HttpUtility.UrlDecode(request.Cookies[cookieName]), this.passPhrase, this.salt);
                var parts = data.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts.Select(part => part.Split('=')))
                {
                    dictionary[HttpUtility.UrlDecode(part[0])] = HttpUtility.UrlDecode(part[1]);
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