namespace Nancy.Security
{
    using System;
    using Cookies;

    using Nancy.Helpers;

    /// <summary>
    /// Csrf protection methods
    /// </summary>
    public static class Csrf
    {
        /// <summary>
        /// Creates a new csrf token for this response with an optional salt.
        /// Only necessary if a particular route requires a new token for each request.
        /// </summary>
        /// <param name="module">Nancy module</param>
        /// <returns></returns>
        public static void CreateNewCsrfToken(this NancyModule module)
        {
            var token = new CsrfToken
            {
                CreatedDate = DateTime.Now,
            };
            token.CreateRandomBytes();
            token.CreateHmac(CsrfStartup.CryptographyConfiguration.HmacProvider);

            var tokenString = CsrfStartup.ObjectSerializer.Serialize(token);

            module.Context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;
        }

        /// <summary>
        /// Validate that the incoming request has valid CSRF tokens.
        /// Throws <see cref="CsrfValidationException"/> if validation fails.
        /// </summary>
        /// <param name="module">Module object</param>
        /// <param name="validityPeriod">Optional validity period before it times out</param>
        /// <exception cref="CsrfValidationException">If validation fails</exception>
        public static void ValidateCsrfToken(this NancyModule module, TimeSpan? validityPeriod = null)
        {
            var request = module.Request;

            if (request == null)
            {
                return;
            }

            var cookieToken = GetCookieToken(request);
            var formToken = GetFormToken(request);

            var result = CsrfStartup.TokenValidator.Validate(cookieToken, formToken, validityPeriod);

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
                formToken = (CsrfToken)CsrfStartup.ObjectSerializer.Deserialize(formTokenString);
            }

            return formToken;
        }

        private static CsrfToken GetCookieToken(Request request)
        {
            CsrfToken cookieToken = null;

            string cookieTokenString;
            if (request.Cookies.TryGetValue(CsrfToken.DEFAULT_CSRF_KEY, out cookieTokenString))
            {
                cookieToken = (CsrfToken)CsrfStartup.ObjectSerializer.Deserialize(HttpUtility.UrlDecode(cookieTokenString));
            }

            return cookieToken;
        }
    }
}