namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Responses;

    /// <summary>
    /// Hooks to be used in a request pipeline.
    /// </summary>
    public static class SecurityHooks
    {
        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the request was made by an authenticated user.
        /// </summary>
        /// <returns>Hook that returns an Unauthorized response if not authenticated in,
        /// null otherwise</returns>
        public static Func<NancyContext, Response> RequiresAuthentication()
        {
            return UnauthorizedIfNot(ctx => ctx.CurrentUser.IsAuthenticated());
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure
        /// that the request was made by an authenticated user having the required claims.
        /// </summary>
        /// <param name="claims">Claims the authenticated user needs to have</param>
        /// <returns>Hook that returns an Unauthorized response if the user is not
        /// authenticated or does not have the required claims, null otherwise</returns>
        public static Func<NancyContext, Response> RequiresClaims(IEnumerable<Claim> claims)
        {
            return ForbiddenIfNot(ctx => ctx.CurrentUser.HasClaims(claims));
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure
        /// that the request was made by an authenticated user having at least one of
        /// the required claims.
        /// </summary>
        /// <param name="claims">Claims the authenticated user needs to have at least one of</param>
        /// <returns>Hook that returns an Unauthorized response if the user is not
        /// authenticated or does not have at least one of the required claims, null
        /// otherwise</returns>
        public static Func<NancyContext, Response> RequiresAnyClaim(IEnumerable<Claim> claims)
        {
            return ForbiddenIfNot(ctx => ctx.CurrentUser.HasAnyClaim(claims));
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure
        /// that the request was made by an authenticated user whose claims satisfy the
        /// supplied validation function.
        /// </summary>
        /// <param name="isValid">Validation function to be called with the authenticated
        /// users claims</param>
        /// <returns>Hook that returns an Unauthorized response if the user is not
        /// authenticated or does not pass the supplied validation function, null
        /// otherwise</returns>
        public static Func<NancyContext, Response> RequiresValidatedClaims(Func<IEnumerable<Claim>, bool> isValid)
        {
            return ForbiddenIfNot(ctx => ctx.CurrentUser.HasValidClaims(isValid));
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the request satisfies a specific test.
        /// </summary>
        /// <param name="test">Test that must return true for the request to continue</param>
        /// <returns>Hook that returns an Unauthorized response if the test fails, null otherwise</returns>
        private static Func<NancyContext, Response> UnauthorizedIfNot(Func<NancyContext, bool> test)
        {
            return HttpStatusCodeIfNot(HttpStatusCode.Unauthorized, test);
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the request satisfies a specific test.
        /// </summary>
        /// <param name="test">Test that must return true for the request to continue</param>
        /// <returns>Hook that returns an Forbidden response if the test fails, null otherwise</returns>
        private static Func<NancyContext, Response> ForbiddenIfNot(Func<NancyContext, bool> test)
        {
            return HttpStatusCodeIfNot(HttpStatusCode.Forbidden, test);
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the request satisfies a specific test.
        /// </summary>
        /// <param name="statusCode">HttpStatusCode to use for the response</param>
        /// <param name="test">Test that must return true for the request to continue</param>
        /// <returns>Hook that returns a response with a specific HttpStatusCode if the test fails, null otherwise</returns>
        private static Func<NancyContext, Response> HttpStatusCodeIfNot(HttpStatusCode statusCode, Func<NancyContext, bool> test)
        {
            return (ctx) =>
            {
                Response response = null;
                if (!test(ctx))
                {
                    response = new Response { StatusCode = statusCode };
                }

                return response;
            };
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the resource is served over HTTPS
        /// </summary>
        /// <param name="redirect"><see langword="true"/> if the user should be redirected to HTTPS (no port number) if the incoming request was made using HTTP, otherwise <see langword="false"/> if <see cref="HttpStatusCode.Forbidden"/> should be returned.</param>
        /// <returns>Hook that returns a RedirectResponse with the Url scheme set to HTTPS,
        /// or a Response with a Forbidden HTTP status code if <c>redirect</c> is false or the method is not GET,
        /// null otherwise</returns>
        public static Func<NancyContext, Response> RequiresHttps(bool redirect)
        {
            return RequiresHttps(redirect, null);
        }

        /// <summary>
        /// Creates a hook to be used in a pipeline before a route handler to ensure that
        /// the resource is served over HTTPS
        /// </summary>
        /// <param name="redirect"><see langword="true"/> if the user should be redirected to HTTPS (no port number) if the incoming request was made using HTTP, otherwise <see langword="false"/> if <see cref="HttpStatusCode.Forbidden"/> should be returned.</param>
        /// <param name="httpsPort">The HTTPS port number to use</param>
        /// <returns>Hook that returns a <see cref="RedirectResponse"/> with the Url scheme set to HTTPS,
        /// or a <see cref="Response"/> with a <see cref="HttpStatusCode.Forbidden"/> status code if <c>redirect</c> is false or the method is not GET,
        /// null otherwise</returns>
        public static Func<NancyContext, Response> RequiresHttps(bool redirect, int? httpsPort)
        {
            return (ctx) =>
            {
                Response response = null;
                var request = ctx.Request;
                if (!request.Url.IsSecure)
                {
                    if (redirect && request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        var redirectUrl = request.Url.Clone();
                        redirectUrl.Port = httpsPort;
                        redirectUrl.Scheme = "https";
                        response = new RedirectResponse(redirectUrl.ToString());
                    }
                    else
                    {
                        response = new Response { StatusCode = HttpStatusCode.Forbidden };
                    }
                }

                return response;
            };
        }
    }
}