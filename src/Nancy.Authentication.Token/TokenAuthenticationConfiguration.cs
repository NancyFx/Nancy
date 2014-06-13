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
        /// <param name="userMapper">The user mapper.</param>
        public TokenAuthenticationConfiguration(ITokenizer tokenizer, IUserMapper userMapper = null)
        {
            if (tokenizer == null)
            {
                throw new ArgumentNullException("tokenizer");
            }

            this.Tokenizer = tokenizer;
            this.UserMapper = userMapper ?? new DefaultUserMapper();
        }

        /// <summary>
        /// Gets the token validator
        /// </summary>
        public ITokenizer Tokenizer { get; private set; }

        public IUserMapper UserMapper { get; set; }
    }
}