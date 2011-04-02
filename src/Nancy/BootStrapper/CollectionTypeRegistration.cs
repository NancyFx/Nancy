namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a type to be registered multiple times into the
    /// container to later be resolved using an IEnumerable{RegistrationType}
    /// constructor dependency.
    /// </summary>
    public class CollectionTypeRegistration
    {
        /// <summary>
        /// Represents a type to be registered multiple times into the
        /// container to later be resolved using an IEnumerable{RegistrationType}
        /// constructor dependency.
        /// </summary>
        /// <param name="registrationType">Registration type i.e. IMyInterface</param>
        /// <param name="implementationTypes">Collection of implementation type i.e. MyClassThatImplementsIMyInterface</param>
        public CollectionTypeRegistration(Type registrationType, IEnumerable<Type> implementationTypes)
        {
            this.RegistrationType = registrationType;
            this.ImplementationTypes = implementationTypes;
        }

        /// <summary>
        /// Registration type i.e. IMyInterface
        /// </summary>
        public Type RegistrationType { get; private set; }

        /// <summary>
        /// Collection of implementation type i.e. MyClassThatImplementsIMyInterface
        /// </summary>
        public IEnumerable<Type> ImplementationTypes { get; private set; }
    }
}