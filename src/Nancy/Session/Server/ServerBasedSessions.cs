namespace Nancy.Session.Server
{
    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Server session implementation based OWASP reccomendations <see cref="https://www.owasp.org/index.php/Session_Management_Cheat_Sheet"/>
    /// </summary>
    public class ServerBasedSessions : IObjectSerializerSelector
    {
        public const int SID_SIZE = 64;

        IEncryptionProvider _encryptionProvider;
        IHmacProvider _hmacProvider;
        SessionStorageProvider _storageProvider;
        RNGCryptoServiceProvider _rng;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBasedSessions"/> class.
        /// </summary>
        /// <param name="encryptionProvider">The encryption provider.</param>
        /// <param name="hmacProvider">The hmac provider</param>
        /// <param name="objectSerializer">Session object serializer to use</param>
        /// <param name="storageProvider">The session persistent storage provider, uses <see cref="DefaultSessionStorageProvider"/> if null</param>
        public ServerBasedSessions(IEncryptionProvider encryptionProvider, IHmacProvider hmacProvider, SessionStorageProvider storageProvider=null)
        {
            _rng = new RNGCryptoServiceProvider();
            _encryptionProvider = encryptionProvider;
            _hmacProvider = hmacProvider;

            if (storageProvider == null)
            {
                _storageProvider = new DefaultSessionStorageProvider();
            }
            else
            {
                _storageProvider = storageProvider;
            }
        }

        /// <summary>
        /// Initialise and add server based session hooks to the application pipeine
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Enable(IPipelines pipelines, CryptographyConfiguration cryptoConfig, SessionStorageProvider storageProvider = null)
        {
            var sessionStore = new ServerBasedSessions(cryptoConfig.EncryptionProvider, cryptoConfig.HmacProvider, storageProvider);

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, sessionStore));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionStore));
        }

        /// <summary>
        /// Save the session into the response
        /// </summary>
        /// <param name="session">Session to save</param>
        /// <param name="context">Nancy Context</param>
        public void Save(ISession session, NancyContext context)
        {
            if (session == null || !session.HasChanged)
            {
                return;
            }
            string SIDString = null;
            if(!context.Request.Cookies.ContainsKey("N_SID"))
            {
                byte[] SID = new byte[SID_SIZE];
                _rng.GetBytes(SID);
                SIDString = BitConverter.ToString(SID);
                var encryptedCookie = _encryptionProvider.Encrypt(SIDString);
                var hmacBytes = _hmacProvider.GenerateHmac(encryptedCookie);
                var cookieData = String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedCookie);
                context.Response.WithCookie("N_SID", cookieData);
            }
            else
            {
                var cookieData = HttpUtility.UrlDecode(context.Request.Cookies["N_SID"]);
                var hmacLength = Base64Helpers.GetBase64Length(_hmacProvider.HmacLength);
                var hmacString = cookieData.Substring(0, hmacLength);
                var encryptedCookie = cookieData.Substring(hmacLength);
                SIDString = _encryptionProvider.Decrypt(encryptedCookie);

            }

            _storageProvider.SaveSession(SIDString, session);
        }

        /// <summary>
        /// Loads the session from the request
        /// </summary>
        /// <returns>ISession containing the load session values</returns>
        public ISession Load(NancyContext context)
        {
            ISession session = null;
            if (context.Request.Cookies.ContainsKey("N_SID"))//returned visit
            {
                var cookieData = HttpUtility.UrlDecode(context.Request.Cookies["N_SID"]);
                var hmacLength = Base64Helpers.GetBase64Length(_hmacProvider.HmacLength);
                var hmacString = cookieData.Substring(0, hmacLength);
                var encryptedCookie = cookieData.Substring(hmacLength);

                var hmacBytes = Convert.FromBase64String(hmacString);
                var newHmac = _hmacProvider.GenerateHmac(encryptedCookie);
                var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, _hmacProvider.HmacLength);

                if (hmacValid)
                {
                    var encryptedSIDString = cookieData.Substring(hmacLength);
                    string sidString = _encryptionProvider.Decrypt(encryptedSIDString);
                    session = _storageProvider.GetSession(sidString);
                }
                else
                {
                    context.Request.Cookies.Remove("N_SID");
                }
            }

            if (session == null)//first visit or invalid sid
            {
                session=new Session();
            }

            return session;
        }

        /// <summary>
        /// Saves the request session into the response
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        private static void SaveSession(NancyContext context, ServerBasedSessions sessionStore)
        {
            sessionStore.Save(context.Request.Session, context);
        }

        /// <summary>
        /// Loads the request session
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="sessionStore">Session store</param>
        /// <returns>Always returns null</returns>
        private static Response LoadSession(NancyContext context, ServerBasedSessions sessionStore)
        {
            if (context.Request == null)
            {
                return null;
            }

            context.Request.Session = sessionStore.Load(context);

            return null;
        }

        public void WithSerializer(IObjectSerializer newSerializer)
        {
        }
    }
}
