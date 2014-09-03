namespace Nancy.Validation
{
    using System;

    /// <summary>
    /// Exception that is thrown during problems with model validation.
    /// </summary>
    public class ModelValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationException"/> class.
        /// </summary>
        public ModelValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationException"/> class,
        /// with the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ModelValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelValidationException"/> class,
        /// with the provided <paramref name="message"/> and <paramref name="innerException"/>
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ModelValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}