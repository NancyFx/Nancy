namespace NancyAuthenticationDemo
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Extensions;
    using Nancy;
    using Nancy.Responses;

    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            this.BeforeRequest += (ctx) =>
            {
                // World's-worse-authentication (TM)
                // Pull the username out of the querystring if it exists
                // and build claims from it
                var username = ctx.Request.Query.username;

                if (username.HasValue)
                {
                    ctx.Items[SecurityExtensions.USERNAME_KEY] = username.ToString();
                    ctx.Items[SecurityExtensions.CLAIMS_KEY] = BuildClaims(username.ToString());
                }

                return null;
            };

            this.AfterRequest += (ctx) =>
            {
                // If status code comes back as Unauthorized then
                // forward the user to the login page
                if (ctx.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ctx.Response = new RedirectResponse("/login?returnUrl=" + ctx.Request.Uri);
                }
            };
        }

        /// <summary>
        /// Build claims based on username
        /// </summary>
        /// <param name="userName">Current username</param>
        /// <returns>IEnumerable of claims</returns>
        private static IEnumerable<string> BuildClaims(string userName)
        {
            var claims = new List<string>();

            // Only bob can have access to SuperSecure
            if (String.Equals(userName, "bob", StringComparison.InvariantCultureIgnoreCase))
            {
                claims.Add("SuperSecure");
            }

            return claims;
        }
    }
}