namespace Nancy.Security
{
    using System;

    public class CsrfValidationException : Exception
    {
        public CsrfValidationException(CsrfTokenValidationResult result)
            : base(result.ToString())
        {
            Result = result;
        }

        public CsrfTokenValidationResult Result { get; private set; }
    }
}