namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
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
        public static Func<NancyContext, Response> RequiresClaims(IEnumerable<string> claims)
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
        public static Func<NancyContext, Response> RequiresAnyClaim(IEnumerable<string> claims)
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
        public static Func<NancyContext, Response> RequiresValidatedClaims(Func<IEnumerable<string>, bool> isValid)
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

        public static Func<NancyContext, Response> RequiresHttps(bool redirect)
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