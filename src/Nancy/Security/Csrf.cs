namespace Nancy.Security
{
    using System;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Cookies;
    using Nancy.Cryptography;
    using Nancy.Helpers;

    /// <summary>
    /// Csrf protection methods
    /// </summary>
    public static class Csrf
    {
        private const string CsrfHookName = "CsrfPostHook";

        /// <summary>
        /// Enables Csrf token generation.
        /// This is disabled by default.
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Enable(IPipelines pipelines, CryptographyConfiguration cryptographyConfiguration = null)
        {
            cryptographyConfiguration = cryptographyConfiguration ?? CsrfApplicationStartup.CryptographyConfiguration;

            var postHook = new PipelineItem<Action<NancyContext>>(
                CsrfHookName,
                context =>
                {
                    if (context.Response == null || context.Response.Cookies == null || context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    if (context.Items.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY))
                    {
                        context.Response.Cookies.Add(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY,
                                                                     (string)context.Items[CsrfToken.DEFAULT_CSRF_KEY],
                                                                     true));
                        return;
                    }

                    if (context.Request.Cookies.ContainsKey(CsrfToken.DEFAULT_CSRF_KEY))
                    {
                        var cookieValue = context.Request.Cookies[CsrfToken.DEFAULT_CSRF_KEY];
                        var cookieToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(cookieValue) as CsrfToken;

                        if (CsrfApplicationStartup.TokenValidator.CookieTokenStillValid(cookieToken))
                        {
                            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = cookieValue;
                            return;
                        }
                    }

                    var tokenString = GenerateTokenString(cryptographyConfiguration);

                    context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;
                    context.Response.Cookies.Add(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY, tokenString, true));
                });

            pipelines.AfterRequest.AddItemToEndOfPipeline(postHook);
        }

        /// <summary>
        /// Disable csrf token generation
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public static void Disable(IPipelines pipelines)
        {
            pipelines.AfterRequest.RemoveByName(CsrfHookName);
        }

        /// <summary>
        /// Creates a new csrf token for this response with an optional salt.
        /// Only necessary if a particular route requires a new token for each request.
        /// </summary>
        /// <param name="module">Nancy module</param>
        /// <returns></returns>
        public static void CreateNewCsrfToken(this INancyModule module, CryptographyConfiguration cryptographyConfiguration = null)
        {
            var tokenString = GenerateTokenString(cryptographyConfiguration);
            module.Context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;
        }

        /// <summary>
        /// Creates a new csrf token with an optional salt.
        /// Does not store the token in context.
        /// </summary>
        /// <returns>The generated token</returns>
        internal static string GenerateTokenString(CryptographyConfiguration cryptographyConfiguration = null)
        {
            cryptographyConfiguration = cryptographyConfiguration ?? CsrfApplicationStartup.CryptographyConfiguration;
            var token = new CsrfToken
            {
                CreatedDate = DateTime.Now,
            };
            token.CreateRandomBytes();
            token.CreateHmac(cryptographyConfiguration.HmacProvider);
            var tokenString = CsrfApplicationStartup.ObjectSerializer.Serialize(token);
            return tokenString;
        }

        /// <summary>
        /// Validate that the incoming request has valid CSRF tokens.
        /// Throws <see cref="CsrfValidationException"/> if validation fails.
        /// </summary>
        /// <param name="module">Module object</param>
        /// <param name="validityPeriod">Optional validity period before it times out</param>
        /// <exception cref="CsrfValidationException">If validation fails</exception>
        public static void ValidateCsrfToken(this INancyModule module, TimeSpan? validityPeriod = null)
        {
            var request = module.Request;

            if (request == null)
            {
                return;
            }

            var cookieToken = GetCookieToken(request);
            var providedToken = GetProvidedToken(request);

            var result = CsrfApplicationStartup.TokenValidator.Validate(cookieToken, providedToken, validityPeriod);

            if (result != CsrfTokenValidationResult.Ok)
            {
                throw new CsrfValidationException(result);
            }
        }

        private static CsrfToken GetProvidedToken(Request request)
        {
            CsrfToken providedToken = null;

            var providedTokenString = request.Form[CsrfToken.DEFAULT_CSRF_KEY].Value ?? request.Headers[CsrfToken.DEFAULT_CSRF_KEY].FirstOrDefault();
            if (providedTokenString != null)
            {
                providedToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(providedTokenString) as CsrfToken;
            }

            return providedToken;
        }

        private static CsrfToken GetCookieToken(Request request)
        {
            CsrfToken cookieToken = null;

            string cookieTokenString;
            if (request.Cookies.TryGetValue(CsrfToken.DEFAULT_CSRF_KEY, out cookieTokenString))
            {
                cookieToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(cookieTokenString) as CsrfToken;
            }

            return cookieToken;
        }
    }
}