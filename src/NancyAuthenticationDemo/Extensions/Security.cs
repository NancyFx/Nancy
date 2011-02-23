namespace NancyAuthenticationDemo.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;

    /// <summary>
    /// Some simple helpers give some nice authentication syntax in the modules.
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// Context key that contains the username
        /// </summary>
        public const string USERNAME_KEY = "username";

        /// <summary>
        /// Context key containing the current user's claims
        /// </summary>
        public const string CLAIMS_KEY = "claims";

        /// <summary>
        /// Ensure that the module requires authentication.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Unauthorized response if not logged in, null otherwise</returns>
        public static Response RequiresAuthentication(NancyContext context)
        {
            return !context.Items.ContainsKey(USERNAME_KEY) ? new Response() { StatusCode = HttpStatusCode.Unauthorized } : null;
        }

        /// <summary>
        /// Gets a request hook for checking claims
        /// </summary>
        /// <param name="claims">Required claims</param>
        /// <returns>Before hook delegate</returns>
        public static Func<NancyContext, Response> RequiresClaims(IEnumerable<string> claims)
        {
            return (ctx) =>
                {
                    var failResponse = new Response() { StatusCode = HttpStatusCode.Forbidden };

                    object userClaimsObject;

                    if (!ctx.Items.TryGetValue(CLAIMS_KEY, out userClaimsObject))
                    {
                        return failResponse;
                    }

                    var userClaims = userClaimsObject as IEnumerable<string>;
                    if (userClaims == null)
                    {
                        return failResponse;
                    }

                    if (claims.Any(claim => !userClaims.Contains(claim)))
                    {
                        return failResponse;
                    }

                    return null;
                };
        }
    }
}