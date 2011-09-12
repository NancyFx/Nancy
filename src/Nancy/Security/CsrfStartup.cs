namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;

    using Bootstrapper;
    using Cookies;
    using Cryptography;

    public class CsrfStartup : IStartup
    {
        public static CryptographyConfiguration CryptographyConfiguration { get; private set; }

        public static IObjectSerializer ObjectSerializer { get; private set; }

        public static ICsrfTokenValidator TokenValidator { get; private set; }

        public CsrfStartup(CryptographyConfiguration cryptographyConfiguration, IObjectSerializer objectSerializer, ICsrfTokenValidator tokenValidator)
        {
            CryptographyConfiguration = cryptographyConfiguration;
            ObjectSerializer = objectSerializer;
            TokenValidator = tokenValidator;
        }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        public void Initialize(IApplicationPipelines pipelines)
        {
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
    }
}