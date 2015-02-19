namespace Nancy.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    /// <summary>
    /// Cookie based session storage
    /// </summary>
    public class CookieBasedSessions : IObjectSerializerSelector
    {
        private readonly CookieBasedSessionsConfiguration currentConfiguration;

        /// <summary>
        /// Gets the cookie name that the session is stored in
        /// </summary>
        /// <value>Cookie name</value>
        public string CookieName
        {
            get
            {
                return this.currentConfiguration.CookieName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessions"/> class.
        /// </summary>
        /// <param name="encryptionProvider">The encryption provider.</param>
        /// <param name="hmacProvider">The hmac provider</param>
        /// <param name="objectSerializer">Session object serializer to use</param>
        public CookieBasedSessions(IEncryptionProvider encryptionProvider, IHmacProvider hmacProvider, IObjectSerializer objectSerializer)
        {
            this.currentConfiguration = new CookieBasedSessionsConfiguration
            {
                Serializer = objectSerializer,
                CryptographyConfiguration = new CryptographyConfiguration(encryptionProvider, hmacProvider)
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieBasedSessions"/> class.
        /// </summary>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        public CookieBasedSessions(CookieBasedSessionsConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }
            this.currentConfiguration = configuration;
        }



        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="configuration">Cookie based sessions configuration.</param>
        /// <returns>Formatter selector for choosing a non-default serializer</returns>
        public static IObjectSerializerSelector Enable(IPipelines pipelines, CookieBasedSessionsConfiguration configuration)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            var sessionStore = new CookieBasedSessions(configuration);

            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => LoadSession(ctx, sessionStore));
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => SaveSession(ctx, sessionStore));

            return sessionStore;
        }

        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeline
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <param name="cryptographyConfiguration">Cryptography configuration</param>
        /// <returns>Formatter selector for choosing a non-default serializer</returns>
        public static IObjectSerializerSelector Enable(IPipelines pipelines, CryptographyConfiguration cryptographyConfiguration)
        {
            var cookieBasedSessionsConfiguration = new CookieBasedSessionsConfiguration(cryptographyConfiguration)
            {
                Serializer = new DefaultObjectSerializer()
            };
            return Enable(pipelines, cookieBasedSessionsConfiguration);
        }

        /// <summary>
        /// Initialise and add cookie based session hooks to the application pipeline with the default encryption provider.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        /// <returns>Formatter selector for choosing a non-default serializer</returns>
        public static IObjectSerializerSelector Enable(IPipelines pipelines)
        {
            return Enable(pipelines, new CookieBasedSessionsConfiguration
            {
                Serializer = new DefaultObjectSerializer()
            });
        }

        /// <summary>
        /// Using the specified serializer
        /// </summary>
        /// <param name="newSerializer">Formatter to use</param>
        public void WithSerializer(IObjectSerializer newSerializer)
        {
            this.currentConfiguration.Serializer = newSerializer;
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

                var objectString = this.currentConfiguration.Serializer.Serialize(kvp.Value);

                sb.Append(HttpUtility.UrlEncode(objectString));
                sb.Append(";");
            }

            var cryptographyConfiguration = this.currentConfiguration.CryptographyConfiguration;
            var encryptedData = cryptographyConfiguration.EncryptionProvider.Encrypt(sb.ToString());
            var hmacBytes = cryptographyConfiguration.HmacProvider.GenerateHmac(encryptedData);
            var cookieData = HttpUtility.UrlEncode(String.Format("{0}{1}", Convert.ToBase64String(hmacBytes), encryptedData));

            var cookie = new NancyCookie(this.currentConfiguration.CookieName, cookieData, true)
            {
                Domain = this.currentConfiguration.Domain,
                Path = this.currentConfiguration.Path
            };
            response.WithCookie(cookie);
        }

        /// <summary>
        /// Loads the session from the request
        /// </summary>
        /// <param name="request">Request to load from</param>
        /// <returns>ISession containing the load session values</returns>
        public ISession Load(Request request)
        {
            var dictionary = new Dictionary<string, object>();

            var cookieName = this.currentConfiguration.CookieName;
            var hmacProvider = this.currentConfiguration.CryptographyConfiguration.HmacProvider;
            var encryptionProvider = this.currentConfiguration.CryptographyConfiguration.EncryptionProvider;

            if (request.Cookies.ContainsKey(cookieName))
            {
                var cookieData = HttpUtility.UrlDecode(request.Cookies[cookieName]);
                var hmacLength = Base64Helpers.GetBase64Length(hmacProvider.HmacLength);
                if (cookieData.Length < hmacLength)
                {
                  return new Session(dictionary);
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
    }
}