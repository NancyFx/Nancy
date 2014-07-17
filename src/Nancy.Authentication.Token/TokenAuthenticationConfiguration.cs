namespace Nancy.Authentication.Token
{
    using System;

    /// <summary>
    /// Configuration options for token authentication
    /// </summary>
    public class TokenAuthenticationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthenticationConfiguration"/> class.
        /// </summary>
        /// <param name="tokenizer">A valid instance of <see cref="ITokenizer"/> class</param>
        /// <param name="userIdentityResolver">The user identity resolver.</param>
        public TokenAuthenticationConfiguration(ITokenizer tokenizer, IUserIdentityResolver userIdentityResolver = null)
        {
            if (tokenizer == null)
            {
                throw new ArgumentNullException("tokenizer");
            }

            this.Tokenizer = tokenizer;
            this.UserIdentityResolver = userIdentityResolver ?? new DefaultUserIdentityResolver();
        }

        /// <summary>
        /// Gets the token validator
        /// </summary>
        public ITokenizer Tokenizer { get; private set; }

        /// <summary>
        /// Gets or sets the user identity resolver
        /// </summary>
        public IUserIdentityResolver UserIdentityResolver { get; set; }
    }
}