namespace Nancy.Authentication.Forms
{
    using System;
    using Bootstrapper;
    using Cookies;
    using Cryptography;
    using Helpers;
    using Nancy.Extensions;

    using Responses;
    using Security;

    /// <summary>
    /// Nancy forms authentication implementation
    /// </summary>
    public static class FormsAuthentication
    {
        /// <summary>
        /// The query string key for storing the return url
        /// </summary>
        private const string REDIRECT_QUERYSTRING_KEY = "returnUrl";

        private static string formsAuthenticationCookieName = "_ncfa";

        // TODO - would prefer not to hold this here, but the redirect response needs it
        private static FormsAuthenticationConfiguration currentConfiguration;

        /// <summary>
        /// Gets or sets the forms authentication cookie name
        /// </summary>
        public static string FormsAuthenticationCookieName
        {
            get
            {
                return formsAuthenticationCookieName;
            }

            set
            {
                formsAuthenticationCookieName = value;
            }
        }

        /// <summary>
        /// Enables forms authentication for the application
        /// </summary>
        /// <param name="pipelines">Pipelines to add handlers to (usually "this")</param>
        /// <param name="configuration">Forms authentication configuration</param>
        public static void Enable(IPipelines pipelines, FormsAuthenticationConfiguration configuration)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }

            currentConfiguration = configuration;

            pipelines.BeforeRequest.AddItemToStartOfPipeline(GetLoadAuthenticationHook(configuration));
            pipelines.AfterRequest.AddItemToEndOfPipeline(GetRedirectToLoginHook(configuration));
        }

        /// <summary>
        /// Creates a response that sets the authentication cookie and redirects
        /// the user back to where they came from.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="userIdentifier">User identifier guid</param>
        /// <param name="cookieExpiry">Optional expiry date for the cookie (for 'Remember me')</param>
        /// <param name="fallbackRedirectUrl">Url to redirect to if none in the querystring</param>
        /// <returns>Nancy response with redirect.</returns>
        public static Response UserLoggedInRedirectResponse(NancyContext context, Guid userIdentifier, DateTime? cookieExpiry = null, string fallbackRedirectUrl = "/")
        {
            var redirectUrl = fallbackRedirectUrl;

            if (context.Request.Query[REDIRECT_QUERYSTRING_KEY].HasValue)
            {
                redirectUrl = context.Request.Query[REDIRECT_QUERYSTRING_KEY];
            }

            var response = context.GetRedirect(redirectUrl);
            var authenticationCookie = BuildCookie(userIdentifier, cookieExpiry, currentConfiguration);
            response.AddCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <param name="userIdentifier">User identifier guid</param>
        /// <param name="cookieExpiry">Optional expiry date for the cookie (for 'Remember me')</param>
        /// <returns>Nancy response with status <see cref="HttpStatusCode.OK"/></returns>
        public static Response UserLoggedInResponse(Guid userIdentifier, DateTime? cookieExpiry = null)
        {
            var response =
                (Response)HttpStatusCode.OK;

            var authenticationCookie = 
                BuildCookie(userIdentifier, cookieExpiry, currentConfiguration);

            response.AddCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="redirectUrl">URL to redirect to</param>
        /// <returns>Nancy response</returns>
        public static Response LogOutAndRedirectResponse(NancyContext context, string redirectUrl)
        {
            var response = context.GetRedirect(redirectUrl);
            var authenticationCookie = BuildLogoutCookie(currentConfiguration);
            response.AddCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns>Nancy response</returns>
        public static Response LogOutResponse()
        {
            var response =
                (Response)HttpStatusCode.OK;

            var authenticationCookie = 
                BuildLogoutCookie(currentConfiguration);

            response.AddCookie(authenticationCookie);

            return response;
        }

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from the cookie.
        /// </summary>
        /// <param name="configuration">Forms authentication configuration to use</param>
        /// <returns>Pre request hook delegate</returns>
        private static Func<NancyContext, Response> GetLoadAuthenticationHook(FormsAuthenticationConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return context =>
                {
                    var userGuid = GetAuthenticatedUserFromCookie(context, configuration);

                    if (userGuid != Guid.Empty)
                    {
                        context.CurrentUser =
                            configuration.UserMapper.GetUserFromIdentifier(userGuid);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Gets the post request hook for redirecting to the login page
        /// </summary>
        /// <param name="configuration">Forms authentication configuration to use</param>
        /// <returns>Post request hook delegate</returns>
        private static Action<NancyContext> GetRedirectToLoginHook(FormsAuthenticationConfiguration configuration)
        {
            return context =>
                {
                    if (context.Response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        context.Response = context.GetRedirect(
                            string.Format("{0}?{1}={2}", 
                            configuration.RedirectUrl, 
                            REDIRECT_QUERYSTRING_KEY,
                            context.ToFullPath("~" + context.Request.Path + HttpUtility.UrlEncode(context.Request.Url.Query))));
                    }
                };
        }

        /// <summary>
        /// Gets the authenticated user GUID from the incoming request cookie if it exists
        /// and is valid.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Returns user guid, or Guid.Empty if not present or invalid</returns>
        private static Guid GetAuthenticatedUserFromCookie(NancyContext context, FormsAuthenticationConfiguration configuration)
        {
            if (!context.Request.Cookies.ContainsKey(formsAuthenticationCookieName))
            {
                return Guid.Empty;
            }

            var cookieValue = DecryptAndValidateAuthenticationCookie(context.Request.Cookies[formsAuthenticationCookieName], configuration);

            Guid returnGuid;
            if (String.IsNullOrEmpty(cookieValue) || !Guid.TryParse(cookieValue, out returnGuid))
            {
                return Guid.Empty;
            }

            return returnGuid;
        }

        /// <summary>
        /// Build the forms authentication cookie
        /// </summary>
        /// <param name="userIdentifier">Authenticated user identifier</param>
        /// <param name="cookieExpiry">Optional expiry date for the cookie (for 'Remember me')</param>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Nancy cookie instance</returns>
        private static INancyCookie BuildCookie(Guid userIdentifier, DateTime? cookieExpiry, FormsAuthenticationConfiguration configuration)
        {
            var cookieContents = EncryptAndSignCookie(userIdentifier.ToString(), configuration);

            var cookie = new NancyCookie(formsAuthenticationCookieName, cookieContents, true) { Expires = cookieExpiry };

            return cookie;
        }

        /// <summary>
        /// Builds a cookie for logging a user out
        /// </summary>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Nancy cookie instance</returns>
        private static INancyCookie BuildLogoutCookie(FormsAuthenticationConfiguration configuration)
        {
            return new NancyCookie(formsAuthenticationCookieName, String.Empty, true) { Expires = DateTime.Now.AddDays(-1) };
        }

        /// <summary>
        /// Encrypt and sign the cookie contents
        /// </summary>
        /// <param name="cookieValue">Plain text cookie value</param>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Encrypted and signed string</returns>
        private static string EncryptAndSignCookie(string cookieValue, FormsAuthenticationConfiguration configuration)
        {
            var encryptedCookie = configuration.CryptographyConfiguration.EncryptionProvider.Encrypt(cookieValue);
            var hmacBytes = GenerateHmac(encryptedCookie, configuration);
            var hmacString = Convert.ToBase64String(hmacBytes);

            return String.Format("{1}{0}", encryptedCookie, hmacString);
        }

        /// <summary>
        /// Generate a hmac for the encrypted cookie string
        /// </summary>
        /// <param name="encryptedCookie">Encrypted cookie string</param>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Hmac byte array</returns>
        private static byte[] GenerateHmac(string encryptedCookie, FormsAuthenticationConfiguration configuration)
        {
            return configuration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedCookie);
        }

        /// <summary>
        /// Decrypt and validate an encrypted and signed cookie value
        /// </summary>
        /// <param name="cookieValue">Encrypted and signed cookie value</param>
        /// <param name="configuration">Current configuration</param>
        /// <returns>Decrypted value, or empty on error or if failed validation</returns>
        private static string DecryptAndValidateAuthenticationCookie(string cookieValue, FormsAuthenticationConfiguration configuration)
        {
            // TODO - shouldn't this be automatically decoded by nancy cookie when that change is made?
            var decodedCookie = Helpers.HttpUtility.UrlDecode(cookieValue);

            var hmacStringLength = Base64Helpers.GetBase64Length(configuration.CryptographyConfiguration.HmacProvider.HmacLength);

            var encryptedCookie = decodedCookie.Substring(hmacStringLength);
            var hmacString = decodedCookie.Substring(0, hmacStringLength);

            var encryptionProvider = configuration.CryptographyConfiguration.EncryptionProvider;

            // Check the hmacs, but don't early exit if they don't match
            var hmacBytes = Convert.FromBase64String(hmacString);
            var newHmac = GenerateHmac(encryptedCookie, configuration);
            var hmacValid = HmacComparer.Compare(newHmac, hmacBytes, configuration.CryptographyConfiguration.HmacProvider.HmacLength);

            var decrypted = encryptionProvider.Decrypt(encryptedCookie);

            // Only return the decrypted result if the hmac was ok
            return hmacValid ? decrypted : String.Empty;
        }

     }
}