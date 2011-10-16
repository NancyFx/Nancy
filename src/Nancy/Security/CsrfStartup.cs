namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bootstrapper;

    using Cryptography;

    using Nancy.Cookies;
    using Nancy.Helpers;

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
            pipelines.AfterRequest.AddItemToEndOfPipeline(
                context =>
                    {
                        if (context.Response == null || context.Response.Cookies == null)
                        {
                            return;
                        }

                        if (context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY))
                        {
                            context.Response.Cookies.Add(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY, (string)context.Items[CsrfToken.DEFAULT_CSRF_KEY], true));
                            return;
                        }

                        if (context.Request.Cookies.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY))
                        {
                            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = HttpUtility.UrlDecode(context.Request.Cookies[CsrfToken.DEFAULT_CSRF_KEY]);
                            return;
                        }

                        var token = new CsrfToken
                        {
                            CreatedDate = DateTime.Now,
                        };
                        token.CreateRandomBytes();
                        token.CreateHmac(CryptographyConfiguration.HmacProvider);
                        var tokenString = ObjectSerializer.Serialize(token);

                        context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;
                        context.Response.Cookies.Add(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY, tokenString, true));
                    });
        }
    }
}