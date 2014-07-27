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
        /// <param name="tokenSource">Where in the request the token should be looked for. Defaults to <see cref="TokenSource.Header"/></param>
        public TokenAuthenticationConfiguration(ITokenizer tokenizer, TokenSource tokenSource = TokenSource.Header)
        {
            if (tokenizer == null)
            {
                throw new ArgumentNullException("tokenizer");
            }

            this.Tokenizer = tokenizer;
            this.TokenSource = tokenSource;
        }

        /// <summary>
        /// Gets the token validator
        /// </summary>
        public ITokenizer Tokenizer { get; private set; }

        /// <summary>
        /// Gets the token location on the request
        /// </summary>
        public TokenSource TokenSource { get; set; }
    }
}