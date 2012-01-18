namespace Nancy.Security
{
    using System.Collections.Generic;
    using Bootstrapper;

    using Cryptography;

    public class CsrfStartup : IStartup
    {
        public CsrfStartup(CryptographyConfiguration cryptographyConfiguration, IObjectSerializer objectSerializer, ICsrfTokenValidator tokenValidator)
        {
            CryptographyConfiguration = cryptographyConfiguration;
            ObjectSerializer = objectSerializer;
            TokenValidator = tokenValidator;
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                return null;
            }
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
            Csrf.Enable(pipelines);
        }
    }
}