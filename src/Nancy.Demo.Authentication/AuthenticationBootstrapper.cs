namespace Nancy.Demo.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Bootstrapper;
    using Nancy.Responses;
    using Nancy.TinyIoc;

    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // In reality you would use a pre-built authentication/claims provider
            pipelines.BeforeRequest += ctx =>
            {
                // World's-worse-authentication (TM)
                // Pull the username out of the querystring if it exists
                // and build claims from it
                var username = ctx.Request.Query.username;

                if (username.HasValue)
                {
                    ctx.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(BuildClaims(username), "querystring"));
                }

                return null;
            };

            pipelines.AfterRequest += ctx =>
            {
                // If status code comes back as Unauthorized then
                // forward the user to the login page
                if (ctx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ctx.Response = new RedirectResponse("/login?returnUrl=" + Uri.EscapeDataString(ctx.Request.Path));
                }
            };
        }

        /// <summary>
        /// Build claims based on username
        /// </summary>
        /// <param name="userName">Current username</param>
        /// <returns>IEnumerable of claims</returns>
        private static IEnumerable<Claim> BuildClaims(string userName)
        {
            var claims = new List<Claim>();

            // Only bob can have access to SuperSecure
            if (String.Equals(userName, "bob", StringComparison.OrdinalIgnoreCase))
            {
                claims.Add(new Claim(ClaimTypes.Role, "SuperSecure"));
            }

            return claims;
        }
    }
}