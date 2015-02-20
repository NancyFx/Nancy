namespace Nancy.Authentication.Token
{
    using System.Security.Claims;

    /// <summary>
    /// Encodes and decodes authorization tokens. 
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Create a token from a <see cref="ClaimsPrincipal"/>
        /// </summary>
        /// <param name="claimsPrincipal">The user identity from which to create a token.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <returns>The generated token.</returns>
        string Tokenize(ClaimsPrincipal claimsPrincipal, NancyContext context);

        /// <summary>
        /// Create a <see cref="ClaimsPrincipal"/> from a token
        /// </summary>
        /// <param name="token">The token from which to create a user identity.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <param name="userIdentityResolver">The user identity resolver.</param>
        /// <returns>The detokenized user identity.</returns>
        ClaimsPrincipal Detokenize(string token, NancyContext context, IUserIdentityResolver userIdentityResolver);
    }
}