namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Exception raised when the <see cref="NancyBootstrapperBase{T}"/> discovers more than one
    /// <see cref="IRootPathProvider"/> implementation in the loaded assemblies.
    /// </summary>
    public class MultipleRootPathProvidersLocatedException : BootstrapperException
    {
        private const string DefaultMessageIntroduction = @"More than one IRootPathProvider was found";
        private const string DefaultMessageConclusion = @"and since we do not know which one you want to use, you need to override the RootPathProvider property on your bootstrapper and specify which one to use. Sorry for the inconvenience.";
        private const string DefaultMessage = DefaultMessageIntroduction + ", " + DefaultMessageConclusion;
        private string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        public MultipleRootPathProvidersLocatedException()
            : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MultipleRootPathProvidersLocatedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MultipleRootPathProvidersLocatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        /// <param name="providerTypes">The provider types.</param>
        public MultipleRootPathProvidersLocatedException(IEnumerable<Type> providerTypes)
            : base(DefaultMessage)
        {
            this.StoreProviderTypes(providerTypes);
        }

#if !NETSTANDARD1_6
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleRootPathProvidersLocatedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected MultipleRootPathProvidersLocatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Gets the provider types.
        /// </summary>
        /// <value>
        /// The provider types.
        /// </value>
        public IEnumerable<Type> ProviderTypes { get; internal set; }

        /// <summary>
        /// Stores the provider types.
        /// </summary>
        /// <param name="providerTypes">The provider types.</param>
        private void StoreProviderTypes(IEnumerable<Type> providerTypes)
        {
            this.ProviderTypes =
                providerTypes.ToList().AsReadOnly();

            this.Data.Add("ProviderTypes", this.ProviderTypes);
        }

        /// <summary>
        /// Returns a more friendly and informative message if the list of providers is available.
        /// </summary>
        /// <remarks>
        /// Message generated will be of the format:
        /// <example>
        /// More than one IRootPathProvider was found:
        ///    Nancy.Tests.Functional.Tests.CustomRootPathProvider2
        ///    Nancy.Tests.Functional.Tests.CustomRootPathProvider
        /// and since we do not know which one you want to use, you need to override the RootPathProvider property on your bootstrapper and specify which one to use. Sorry for the inconvenience.
        /// </example>
        /// </remarks>
        public override string Message
        {
            get
            {
                return (this.errorMessage ?? (this.errorMessage = this.GetErrorMessage()));
            }
        }

        private string GetErrorMessage()
        {
            if ((this.ProviderTypes == null) || (!this.ProviderTypes.Any()))
            {
                return base.Message;
            }

            var builder =
                new StringBuilder(DefaultMessageIntroduction);

            foreach (var providerType in this.ProviderTypes)
            {
                builder.AppendFormat("\n    {0}", providerType.FullName);
            }

            builder.AppendFormat("\n {0}", DefaultMessageConclusion);

            return builder.ToString();
        }
    }
}
