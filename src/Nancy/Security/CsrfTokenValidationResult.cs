namespace Nancy.Security
{
    /// <summary>
    /// Result of Csrf Token validation
    /// </summary>
    public enum CsrfTokenValidationResult
    {
        /// <summary>
        /// Validated ok
        /// </summary>
        Ok,

        /// <summary>
        /// One or both of the tokens appears to have been tampered with
        /// </summary>
        TokenTamperedWith,

        /// <summary>
        /// One or both of the tokens are missing
        /// </summary>
        TokenMissing,

        /// <summary>
        /// Tokens to not match
        /// </summary>
        TokenMismatch,

        /// <summary>
        /// Token is valid, but has expired
        /// </summary>
        TokenExpired,
    }
}