namespace Nancy.Authentication.Token
{
    using Nancy.Security;

    /// <summary>
    /// Encodes and decodes authorization tokens. 
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Create a token from a <see cref="IUserIdentity"/>
        /// </summary>
        /// <param name="userIdentity">The user identity from which to create a token.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <returns>The generated token.</returns>
        string Tokenize(IUserIdentity userIdentity, NancyContext context);

        /// <summary>
        /// Create a <see cref="IUserIdentity"/> from a token
        /// </summary>
        /// <param name="token">The token from which to create a user identity.</param>
        /// <param name="context">Current <see cref="NancyContext"/>.</param>
        /// <param name="userIdentityResolver">The user identity resolver.</param>
        /// <returns>The detokenized user identity.</returns>
        IUserIdentity Detokenize(string token, NancyContext context, IUserIdentityResolver userIdentityResolver);
    }
}