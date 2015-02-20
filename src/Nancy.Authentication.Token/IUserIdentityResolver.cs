namespace Nancy.Authentication.Token
{
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Security;

    /// <summary>
    /// Provides a mapping between username and an <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public interface IUserIdentityResolver
    {
        /// <summary>
        /// Gets the <see cref="IUserIdentity"/> from username and claims.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <returns>A populated <see cref="IUserIdentity"/>, or <c>null</c></returns>
        ClaimsPrincipal GetUser(string userName, IEnumerable<Claim> claims, NancyContext context);
    }
}