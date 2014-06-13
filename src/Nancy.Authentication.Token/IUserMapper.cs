namespace Nancy.Authentication.Token
{
    using System.Collections.Generic;

    using Nancy.Security;

    /// <summary>
    /// Provides a mapping between username and an <see cref="IUserIdentity"/>.
    /// </summary>
    public interface IUserMapper
    {
        /// <summary>
        /// Gets the <see cref="IUserIdentity"/> from username and claims.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <returns>A populated <see cref="IUserIdentity"/>, or <c>null</c></returns>
        IUserIdentity GetUser(string userName, IEnumerable<string> claims, NancyContext context);
    }
}