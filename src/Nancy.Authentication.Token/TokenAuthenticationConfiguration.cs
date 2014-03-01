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
        public TokenAuthenticationConfiguration(ITokenizer tokenizer)
        {
            if (tokenizer == null)
            {
                throw new ArgumentNullException("tokenizer");
            }

            this.Tokenizer = tokenizer;
        }

        /// <summary>
        /// Gets the token validator
        /// </summary>
        public ITokenizer Tokenizer { get; private set; }
    }
}