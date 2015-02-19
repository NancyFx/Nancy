namespace Nancy.Security
{
    using System;
    using System.Linq;

    using Nancy.Cryptography;

    /// <summary>
    /// The default implementation of the <see cref="ICsrfTokenValidator"/> interface.
    /// </summary>
    public class DefaultCsrfTokenValidator : ICsrfTokenValidator
    {
        private readonly IHmacProvider hmacProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCsrfTokenValidator"/> class,
        /// using the provided <paramref name="cryptoConfig"/>.
        /// </summary>
        /// <param name="cryptoConfig">The <see cref="CryptographyConfiguration"/> that should be used.</param>
        public DefaultCsrfTokenValidator(CryptographyConfiguration cryptoConfig)
        {
            this.hmacProvider = cryptoConfig.HmacProvider;
        }

        /// <summary>
        /// Validates a pair of tokens
        /// </summary>
        /// <param name="tokenOne">First token (usually from either a form post or querystring)</param>
        /// <param name="tokenTwo">Second token (usually from a cookie)</param>
        /// <param name="validityPeriod">Optional period that the tokens are valid for</param>
        /// <returns>Token validation result</returns>
        public CsrfTokenValidationResult Validate(CsrfToken tokenOne, CsrfToken tokenTwo, TimeSpan? validityPeriod = new TimeSpan?())
        {
            if (tokenOne == null || tokenTwo == null)
            {
                return CsrfTokenValidationResult.TokenMissing;
            }

            if (!tokenOne.Equals(tokenTwo))
            {
                return CsrfTokenValidationResult.TokenMismatch;
            }

            if (tokenOne.RandomBytes == null || tokenOne.RandomBytes.Length == 0)
            {
                return CsrfTokenValidationResult.TokenTamperedWith;
            }

            var newToken = new CsrfToken
                               {
                                   CreatedDate = tokenOne.CreatedDate,
                                   RandomBytes = tokenOne.RandomBytes,
                               };
            newToken.CreateHmac(this.hmacProvider);
            if (!newToken.Hmac.SequenceEqual(tokenOne.Hmac))
            {
                return CsrfTokenValidationResult.TokenTamperedWith;
            }

            if (validityPeriod.HasValue)
            {
                var expiryDate = tokenOne.CreatedDate.Add(validityPeriod.Value);

                if (DateTime.Now > expiryDate)
                {
                    return CsrfTokenValidationResult.TokenExpired;
                }
            }

            return CsrfTokenValidationResult.Ok;
        }

        /// <summary>
        /// Validates that a cookie token is still valid with the current configuration / keys
        /// </summary>
        /// <param name="cookieToken">Token to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool CookieTokenStillValid(CsrfToken cookieToken)
        {
            if (cookieToken == null || cookieToken.RandomBytes == null || cookieToken.RandomBytes.Length == 0)
            {
                return false;
            }

            var newToken = new CsrfToken
            {
                CreatedDate = cookieToken.CreatedDate,
                RandomBytes = cookieToken.RandomBytes,
            };
            newToken.CreateHmac(this.hmacProvider);

            if (!newToken.Hmac.SequenceEqual(cookieToken.Hmac))
            {
                return false;
            }

            return true;
        }
    }
}