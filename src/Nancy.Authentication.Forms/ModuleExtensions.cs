namespace Nancy.Authentication.Forms
{
    using System;

    /// <summary>
    /// Module extensions for login/logout of forms auth
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Logs the user in with the given user guid and redirects
        /// </summary>
        /// <param name="module">Nancy module</param>
        /// <param name="userIdentifier">User identifier guid</param>
        /// <param name="cookieExpiry">Optional expiry date for the cookie (for 'Remember me')</param>
        /// <param name="fallbackRedirectUrl">Url to redirect to if none in the querystring</param>
        /// <returns>Nancy response instance</returns>
        public static Response LoginAndRedirect(this NancyModule module, Guid userIdentifier, DateTime? cookieExpiry = null, string fallbackRedirectUrl = "/")
        {
            return FormsAuthentication.UserLoggedInRedirectResponse(module.Context, userIdentifier, cookieExpiry, fallbackRedirectUrl);
        }

        /// <summary>
        /// Logs the user out and redirects
        /// </summary>
        /// <param name="module">Nancy module</param>
        /// <param name="redirectUrl">URL to redirect to</param>
        /// <returns>Nancy response instance</returns>
        public static Response LogoutAndRedirect(this NancyModule module, string redirectUrl)
        {
            return FormsAuthentication.LogOutAndRedirectResponse(module.Context, redirectUrl);
        }
    }
}