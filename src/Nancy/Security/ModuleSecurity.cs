namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Extensions;
    using Nancy.Responses;

    /// <summary>
    /// Some simple helpers give some nice authentication syntax in the modules.
    /// </summary>
    public static class ModuleSecurity
    {
        /// <summary>
        /// This module requires authentication
        /// </summary>
        /// <param name="module">Module to enable</param>
        public static void RequiresAuthentication(this INancyModule module)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAuthentication(), "Requires Authentication");
        }

        /// <summary>
        /// This module requires authentication and certain claims to be present.
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="requiredClaims">Claim(s) required</param>
        public static void RequiresClaims(this INancyModule module, IEnumerable<Predicate<Claim>> requiredClaims)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAuthentication(), "Requires Authentication");
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresClaims(requiredClaims), "Requires Claims");
        }

        /// <summary>
        /// This module requires authentication and certain claims to be present.
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="requiredClaims">Claim(s) required</param>
        public static void RequiresClaims(this INancyModule module, params string[] requiredClaims)
        {
            module.RequiresClaims(requiredClaims.AsEnumerable());
        }
        
        /// <summary>
        /// This module requires authentication and any one of certain claims to be present.
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="requiredClaims">Claim(s) required</param>
        public static void RequiresAnyClaim(this INancyModule module, IEnumerable<string> requiredClaims)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAuthentication(), "Requires Authentication");
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAnyClaim(requiredClaims), "Requires Any Claim");
        }
        
        /// <summary>
        /// This module requires authentication and any one of certain claims to be present.
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="requiredClaims">Claim(s) required</param>
        public static void RequiresAnyClaim(this INancyModule module, params string[] requiredClaims)
        {
            module.RequiresAnyClaim(requiredClaims.AsEnumerable());
        }

        /// <summary>
        /// This module requires claims to be validated
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="isValid">Claims validator</param>
        public static void RequiresValidatedClaims(this INancyModule module, Func<IEnumerable<Claim>, bool> isValid)
        {
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresAuthentication(), "Requires Authentication");
            module.AddBeforeHookOrExecute(SecurityHooks.RequiresValidatedClaims(isValid), "Requires Validated Claim");
        }

        /// <summary>
        /// This module requires https.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that requires HTTPS.</param>
        public static void RequiresHttps(this INancyModule module)
        {
            module.RequiresHttps(true);
        }

        /// <summary>
        /// This module requires https.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that requires HTTPS.</param>
        /// <param name="redirect"><see langword="true"/> if the user should be redirected to HTTPS (no port number) if the incoming request was made using HTTP, otherwise <see langword="false"/> if <see cref="HttpStatusCode.Forbidden"/> should be returned.</param>
        public static void RequiresHttps(this INancyModule module, bool redirect)
        {
            module.Before.AddItemToEndOfPipeline(SecurityHooks.RequiresHttps(redirect, null));
        }

        /// <summary>
        /// This module requires https.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> that requires HTTPS.</param>
        /// <param name="redirect"><see langword="true"/> if the user should be redirected to HTTPS if the incoming request was made using HTTP, otherwise <see langword="false"/> if <see cref="HttpStatusCode.Forbidden"/> should be returned.</param>
        /// <param name="httpsPort">The HTTPS port number to use</param>
        public static void RequiresHttps(this INancyModule module, bool redirect, int httpsPort)
        {
            module.Before.AddItemToEndOfPipeline(SecurityHooks.RequiresHttps(redirect, httpsPort));
        }
    }
}
