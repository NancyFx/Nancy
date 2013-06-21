using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.Bootstrapper
{
    using System.Runtime.Serialization;

    public class MultipleRootPathProvidersLocatedException : Exception
    {
        private const string defaultMessage = @"Multiple IRootPathProvider implementations have been located.";
        private const string providerInfoMessage = @"See the ProviderTypes property for the list.";

        public MultipleRootPathProvidersLocatedException() : base(defaultMessage)
        {}

        public MultipleRootPathProvidersLocatedException(string message) : base(message)
        {
        }

        public MultipleRootPathProvidersLocatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MultipleRootPathProvidersLocatedException(IEnumerable<Type> providerTypes) : base(defaultMessage + " " + providerInfoMessage)
        {
            this.StoreProviderTypes(providerTypes);
        }

        private void StoreProviderTypes(IEnumerable<Type> providerTypes)
        {
            this.ProviderTypes = providerTypes.ToList().AsReadOnly();
            this.Data.Add("ProviderTypes", this.ProviderTypes);
        }

        public IEnumerable<Type> ProviderTypes { get; internal set; }
 
        protected MultipleRootPathProvidersLocatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
