namespace Nancy.Authentication.Token
{
    using System.Collections.Generic;

    using Nancy.Security;

    /// <summary>
    /// The default user identity resolver.
    /// This creates a plain user identity based on username and claims.
    /// </summary>
    public class DefaultUserIdentityResolver : IUserIdentityResolver
    {
        /// <summary>
        /// Gets the <see cref="IUserIdentity"/> from username and claims.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <returns>A populated <see cref="IUserIdentity"/>, or <c>null</c></returns>
        public IUserIdentity GetUser(string userName, IEnumerable<string> claims, NancyContext context)
        {
            return new TokenUserIdentity(userName, claims);
        }

        private class TokenUserIdentity : IUserIdentity
        {
            public TokenUserIdentity(string userName, IEnumerable<string> claims)
            {
                this.UserName = userName;
                this.Claims = claims;
            }

            public string UserName { get; private set; }

            public IEnumerable<string> Claims { get; private set; }
        }
    }
}