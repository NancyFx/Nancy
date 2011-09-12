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
        /// Add a Csrf protection token to the response
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="forceRecreation">Force the tokens to be regenerated even if the user already has one</param>
        /// <param name="salt">Optional salt - must match verification call</param>
        /// <returns>Modified response object</returns>
        /// <seealso cref="CsrfTokenValidationResult"/>
        public static Response WithCsrfToken(this Response response, bool forceRecreation = false, string salt = null)
        {
            var token = new CsrfToken
                            {
                                CreatedDate = DateTime.Now,
                                Salt = salt ?? string.Empty
                            };
            token.CreateRandomBytes();
            token.CreateHmac(CsrfStartup.CryptographyConfiguration.HmacProvider);

            var tokenString = CsrfStartup.ObjectSerializer.Serialize(token);

            response.AddCookie(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY, tokenString, true));

            return response;
        }

        /// <summary>
        /// Validate that the incoming request has valid CSRF tokens.
        /// Throws <see cref="CsrfValidationException"/> if validation fails.
        /// </summary>
        /// <param name="module">Module object</param>
        /// <param name="validityPeriod">Optional validity period before it times out</param>
        /// <param name="salt">Optional salt - must match the creation call</param>
        /// <exception cref="CsrfValidationException">If validation fails</exception>
        public static void ValidateCsrfToken(this NancyModule module, TimeSpan? validityPeriod = null, string salt = null)
        {
            var request = module.Request;

            if (request == null)
            {
                return;
            }

            var cookieToken = GetCookieToken(request);
            var formToken = GetFormToken(request);

            var result = CsrfStartup.TokenValidator.Validate(cookieToken, formToken, salt, validityPeriod);

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