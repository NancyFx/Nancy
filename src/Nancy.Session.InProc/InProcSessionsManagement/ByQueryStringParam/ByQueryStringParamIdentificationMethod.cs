namespace Nancy.Session.InProc.InProcSessionsManagement.ByQueryStringParam
{
    using System;
    using Nancy.Cryptography;

    /// <summary>
    /// Identification method for in-process memory based sessions, using a querystring parameter, that contains the session identifier.
    /// </summary>
    public class ByQueryStringParamIdentificationMethod : IByQueryStringParamIdentificationMethod
    {
        private const string DefaultParameterName = "_nsid";
        private readonly IEncryptionProvider encryptionProvider;
        private readonly IHmacProvider hmacProvider;
        private readonly IHmacValidator hmacValidator;
        private readonly IResponseManipulatorForSession responseManipulatorForSession;
        private readonly ISessionIdentificationDataProvider sessionIdentificationDataProvider;
        private readonly ISessionIdFactory sessionIdFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByQueryStringParamIdentificationMethod"/> class.
        /// </summary>
        public ByQueryStringParamIdentificationMethod(CryptographyConfiguration cryptoConfig)
        {
            if (cryptoConfig == null)
            {
                throw new ArgumentNullException("cryptoConfig");
            }
            this.encryptionProvider = cryptoConfig.EncryptionProvider;
            this.hmacProvider = cryptoConfig.HmacProvider;
            this.sessionIdentificationDataProvider = new SessionIdentificationDataProvider(cryptoConfig.HmacProvider);
            this.hmacValidator = new HmacValidator(cryptoConfig.HmacProvider);
            this.sessionIdFactory = new SessionIdFactory();
            this.responseManipulatorForSession = new ResponseManipulatorForSession();
            this.ParameterName = DefaultParameterName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ByQueryStringParamIdentificationMethod"/> class.
        /// </summary>
        internal ByQueryStringParamIdentificationMethod(
            IEncryptionProvider encryptionProvider,
            IHmacProvider hmacProvider,
            ISessionIdentificationDataProvider sessionIdentificationDataProvider,
            IHmacValidator hmacValidator,
            ISessionIdFactory sessionIdFactory,
            IResponseManipulatorForSession responseManipulatorForSession)
        {
            if (encryptionProvider == null)
            {
                throw new ArgumentNullException("encryptionProvider");
            }
            if (hmacProvider == null)
            {
                throw new ArgumentNullException("hmacProvider");
            }
            if (sessionIdentificationDataProvider == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (hmacValidator == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (sessionIdFactory == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (responseManipulatorForSession == null)
            {
                throw new ArgumentNullException("responseManipulatorForSession");
            }
            this.encryptionProvider = encryptionProvider;
            this.hmacProvider = hmacProvider;
            this.sessionIdentificationDataProvider = sessionIdentificationDataProvider;
            this.hmacValidator = hmacValidator;
            this.sessionIdFactory = sessionIdFactory;
            this.responseManipulatorForSession = responseManipulatorForSession;
            this.ParameterName = DefaultParameterName;
        }

        /// <summary>
        /// Load the session identifier from the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The identifier of the session for the current request.</returns>
        public SessionId GetCurrentSessionId(NancyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var queryStringData = this.sessionIdentificationDataProvider.ProvideDataFromQuery(context.Request,
                this.ParameterName);
            if (queryStringData == null)
            {
                return this.sessionIdFactory.CreateNew();
            }
            var isHmacValid = this.hmacValidator.IsValidHmac(queryStringData);
            if (!isHmacValid)
            {
                return this.sessionIdFactory.CreateNew();
            }

            var decryptedSessionId = this.encryptionProvider.Decrypt(queryStringData.SessionId);
            if (string.IsNullOrEmpty(decryptedSessionId))
            {
                return this.sessionIdFactory.CreateNew();
            }

            return this.sessionIdFactory.CreateFrom(decryptedSessionId) ?? this.sessionIdFactory.CreateNew();
        }

        /// <summary>
        /// Save the session identifier in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        public void SaveSessionId(SessionId sessionId, NancyContext context)
        {
            if (sessionId == null)
            {
                throw new ArgumentNullException("sessionId");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (context.Request == null)
            {
                throw new ArgumentException("The specified context does not contain a request", "context");
            }
            if (sessionId.IsEmpty)
            {
                throw new ArgumentException("The specified session id cannot be empty", "sessionId");
            }

            // Redirect the client to the same url, with the session Id as a query string parameter, if needed
            if (sessionId.IsNew)
            {
                var encryptedSessionId = this.encryptionProvider.Encrypt(sessionId.Value.ToString());
                var hmacBytes = this.hmacProvider.GenerateHmac(encryptedSessionId);

                var sessionIdentificationData = new SessionIdentificationData
                {
                    SessionId = encryptedSessionId,
                    Hmac = hmacBytes
                };

                this.responseManipulatorForSession.ModifyResponseToRedirectToSessionAwareUrl(
                    context,
                    sessionIdentificationData,
                    this.ParameterName);
            }
        }

        /// <summary>
        /// Gets or sets the querystring parameter name in which the session id is stored.
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }
    }
}