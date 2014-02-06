namespace Nancy.Security
{
    using System;
    using Cookies;
    using Nancy.Bootstrapper;
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
        public static void Enable(IPipelines pipelines)
        {
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
                        var decodedValue = HttpUtility.UrlDecode(context.Request.Cookies[CsrfToken.DEFAULT_CSRF_KEY]);
                        var cookieToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(decodedValue) as CsrfToken;

                        if (CsrfApplicationStartup.TokenValidator.CookieTokenStillValid(cookieToken))
                        {
                            context.Items[CsrfToken.DEFAULT_CSRF_KEY] = decodedValue;
                            return;
                        }
                    }

                    var token = new CsrfToken
                    {
                        CreatedDate = DateTime.Now,
                    };
                    token.CreateRandomBytes();
                    token.CreateHmac(CsrfApplicationStartup.CryptographyConfiguration.HmacProvider);
                    var tokenString = CsrfApplicationStartup.ObjectSerializer.Serialize(token);

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
        public static void CreateNewCsrfToken(this INancyModule module)
        {
            var token = new CsrfToken
            {
                CreatedDate = DateTime.Now,
            };
            token.CreateRandomBytes();
            token.CreateHmac(CsrfApplicationStartup.CryptographyConfiguration.HmacProvider);

            var tokenString = CsrfApplicationStartup.ObjectSerializer.Serialize(token);

            module.Context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;
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
            var formToken = GetFormToken(request);

            var result = CsrfApplicationStartup.TokenValidator.Validate(cookieToken, formToken, validityPeriod);

            if (result != CsrfTokenValidationResult.Ok)
            {
                throw new CsrfValidationException(result);
            }
        }

        private static CsrfToken GetFormToken(Request request)
        {
            CsrfToken formToken = null;
            
            var formTokenString = request.Form[CsrfToken.DEFAULT_CSRF_KEY].Value;
            if (formTokenString != null)
            {
                formToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(formTokenString) as CsrfToken;
            }

            return formToken;
        }

        private static CsrfToken GetCookieToken(Request request)
        {
            CsrfToken cookieToken = null;

            string cookieTokenString;
            if (request.Cookies.TryGetValue(CsrfToken.DEFAULT_CSRF_KEY, out cookieTokenString))
            {
                cookieToken = CsrfApplicationStartup.ObjectSerializer.Deserialize(HttpUtility.UrlDecode(cookieTokenString)) as CsrfToken;
            }

            return cookieToken;
        }
    }
}