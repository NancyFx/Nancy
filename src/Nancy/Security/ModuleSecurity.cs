namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Some simple helpers give some nice authentication syntax in the modules.
    /// </summary>
    public static class ModuleSecurity
    {
        /// <summary>
        /// This module requires authentication
        /// </summary>
        /// <param name="module">Module to enable</param>
        public static void RequiresAuthentication(this NancyModule module)
        {
            module.Before.AddItemToEndOfPipeline(RequiresAuthentication);
        }

        /// <summary>
        /// This module requires authentication and certain claims to be present.
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="requiredClaims">Claim(s) required</param>
        public static void RequiresClaims(this NancyModule module, IEnumerable<string> requiredClaims)
        {
            module.Before.AddItemToEndOfPipeline(RequiresAuthentication);
            module.Before.AddItemToEndOfPipeline(RequiresClaims(requiredClaims));
        }

        /// <summary>
        /// This module requires claims to be validated
        /// </summary>
        /// <param name="module">Module to enable</param>
        /// <param name="isValid">Claims validator</param>
        public static void RequiresValidatedClaims(this NancyModule module, Func<IEnumerable<string>, bool> isValid)
        {
            module.Before.AddItemToStartOfPipeline(RequiresValidatedClaims(isValid));
            module.Before.AddItemToStartOfPipeline(RequiresAuthentication);
        }

        /// <summary>
        /// Ensure that the module requires authentication.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Unauthorized response if not logged in, null otherwise</returns>
        private static Response RequiresAuthentication(NancyContext context)
        {
            Response response = null;
            if ( (context.CurrentUser == null) ||
                String.IsNullOrWhiteSpace(context.CurrentUser.UserName) )
            {
                response = new Response { StatusCode = HttpStatusCode.Unauthorized };
            }

            return response;
        }

        /// <summary>
        /// Gets a request hook for checking claims
        /// </summary>
        /// <param name="claims">Required claims</param>
        /// <returns>Before hook delegate</returns>
        private static Func<NancyContext, Response> RequiresClaims(IEnumerable<string> claims)
        {
            return (ctx) =>
                       {
                           Response response = null;
                           if (ctx.CurrentUser == null
                               || ctx.CurrentUser.Claims == null
                               || claims.Any(c => !ctx.CurrentUser.Claims.Contains(c)))
                           {
                               response = new Response {StatusCode = HttpStatusCode.Forbidden};
                           }

                           return response;
                       };
        }

        /// <summary>
        /// Gets a pipeline item for validating user claims
        /// </summary>
        /// <param name="isValid">Is valid delegate</param>
        /// <returns>Pipeline item delegate</returns>
        private static Func<NancyContext, Response> RequiresValidatedClaims(Func<IEnumerable<string>, bool> isValid)
        {
            return (ctx) =>
                       {
                           Response response = null;
                           var userClaims = ctx.CurrentUser.Claims;
                           if (ctx.CurrentUser == null
                               || ctx.CurrentUser.Claims == null
                               || !isValid(ctx.CurrentUser.Claims))
                           {
                               response = new Response {StatusCode = HttpStatusCode.Forbidden};
                           }

                           return response;
                       };
        }
    }
}