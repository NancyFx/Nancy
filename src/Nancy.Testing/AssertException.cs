namespace Nancy.Testing
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that is thrown by assert extensions.
    /// </summary>
    public class AssertException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AssertException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AssertException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NETSTANDARD1_6
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected AssertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}