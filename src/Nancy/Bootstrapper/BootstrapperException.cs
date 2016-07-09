namespace Nancy.Bootstrapper
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that is raised from inside the <see cref="NancyBootstrapperBase{T}"/> type or one of
    /// the inheriting types.
    /// </summary>
    public class BootstrapperException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperException"/> class, with
        /// the provided <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BootstrapperException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperException"/> class, with
        /// the provided <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public BootstrapperException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NETSTANDARD1_6
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BootstrapperException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
