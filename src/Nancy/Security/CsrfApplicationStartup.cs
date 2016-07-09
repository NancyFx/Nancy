namespace Nancy.Security
{
    using Nancy.Bootstrapper;
    using Nancy.Cryptography;

    /// <summary>
    /// Wires up the CSRF (anti-forgery token) support at application startup.
    /// </summary>
    public class CsrfApplicationStartup : IApplicationStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsrfApplicationStartup"/> class, using the
        /// provided <paramref name="cryptographyConfiguration"/>, <paramref name="objectSerializer"/> and <paramref name="tokenValidator"/>.
        /// </summary>
        /// <param name="cryptographyConfiguration">The cryptographic configuration to use.</param>
        /// <param name="objectSerializer">The serializer that should be used.</param>
        /// <param name="tokenValidator">The token validator that should be used.</param>
        public CsrfApplicationStartup(CryptographyConfiguration cryptographyConfiguration, IObjectSerializer objectSerializer, ICsrfTokenValidator tokenValidator)
        {
            CryptographyConfiguration = cryptographyConfiguration;
            ObjectSerializer = objectSerializer;
            TokenValidator = tokenValidator;
        }

        /// <summary>
        /// Gets the configured crypto config
        /// </summary>
        internal static CryptographyConfiguration CryptographyConfiguration { get; private set; }

        /// <summary>
        /// Gets the configured object serialiser
        /// </summary>
        internal static IObjectSerializer ObjectSerializer { get; private set; }

        /// <summary>
        /// Gets the configured token validator
        /// </summary>
        internal static ICsrfTokenValidator TokenValidator { get; private set; }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
        }
    }
}