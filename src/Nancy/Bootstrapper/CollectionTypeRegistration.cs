namespace Nancy.Bootstrapper
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a type to be registered multiple times into the
    /// container to later be resolved using an IEnumerable{RegistrationType}
    /// constructor dependency.
    /// </summary>
    public class CollectionTypeRegistration : ContainerRegistration
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
            if (registrationType == null)
            {
                throw new ArgumentNullException("registrationType");
            }

            if (implementationTypes == null)
            {
                throw new ArgumentNullException("implementationTypes");
            }

            this.RegistrationType = registrationType;
            this.ImplementationTypes = implementationTypes;

            this.ValidateTypeCompatibility(implementationTypes.ToArray());
        }

        /// <summary>
        /// Collection of implementation type i.e. MyClassThatImplementsIMyInterface
        /// </summary>
        public IEnumerable<Type> ImplementationTypes { get; private set; }
    }
}