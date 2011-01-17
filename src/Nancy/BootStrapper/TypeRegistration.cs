using System;

namespace Nancy.BootStrapper
{
    /// <summary>
    /// Represents a type to be registered into the container
    /// </summary>
    public sealed class TypeRegistration
    {
        /// <summary>
        /// Registration type i.e. IMyInterface
        /// </summary>
        public Type RegistrationType { get; private set; }

        /// <summary>
        /// Implementation type i.e. MyClassThatImplementsIMyInterface
        /// </summary>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Represents a type to be registered into the container
        /// </summary>
        /// <param name="registrationType">Registration type i.e. IMyInterface</param>
        /// <param name="implementationType">Implementation type i.e. MyClassThatImplementsIMyInterface</param>
        public TypeRegistration(Type registrationType, Type implementationType)
        {
            RegistrationType = registrationType;
            ImplementationType = implementationType;
        }
    }
}
