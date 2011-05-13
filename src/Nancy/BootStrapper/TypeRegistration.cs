namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Represents a type to be registered into the container
    /// </summary>
    public sealed class TypeRegistration
    {
        /// <summary>
        /// Represents a type to be registered into the container
        /// </summary>
        /// <param name="registrationType">Registration type i.e. IMyInterface</param>
        /// <param name="implementationType">Implementation type i.e. MyClassThatImplementsIMyInterface</param>
        public TypeRegistration(Type registrationType, Type implementationType)
        {
            if (registrationType == null)
            {
                throw new ArgumentNullException("registrationType");
            }

            if (implementationType == null)
            {
                throw new ArgumentNullException("implementationType");
            }
    
            if (!registrationType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException("implementationType must implement registrationType", "implementationType");    
            }

            this.RegistrationType = registrationType;
            this.ImplementationType = implementationType;
        }

        /// <summary>
        /// Implementation type i.e. MyClassThatImplementsIMyInterface
        /// </summary>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Registration type i.e. IMyInterface
        /// </summary>
        public Type RegistrationType { get; private set; }
    }
}