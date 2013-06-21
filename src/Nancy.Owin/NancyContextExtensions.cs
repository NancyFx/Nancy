namespace Nancy
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Owin;

    public static class NancyContextExtensions
    {
        /// <summary>
        /// Gets the claims principal.
        /// </summary>
        /// <param name="nancyContext">The nancy context.</param>
        /// <returns></returns>
        public static ClaimsPrincipal GetClaimsPrincipal(this NancyContext nancyContext)
        {
            var owinEnvironment = nancyContext.Items.Get<IDictionary<string, object>>(NancyOwinHost.RequestEnvironmentKey);
            return owinEnvironment != null ? owinEnvironment.Get<ClaimsPrincipal>("server.User") : null;
        }
    }
}