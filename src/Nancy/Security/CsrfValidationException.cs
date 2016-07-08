namespace Nancy.Security
{
    using System;

    /// <summary>
    /// Contains the exception information about a CSRF token validation.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CsrfValidationException : Exception
    {
        /// <summary>
        /// Gets the result for the CSRF token validation.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public CsrfTokenValidationResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsrfValidationException"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public CsrfValidationException(CsrfTokenValidationResult result)
            : base(result.ToString())
        {
            Result = result;
        }
    }
}