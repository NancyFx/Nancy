namespace Nancy.Bootstrapper
{
    using System;

    /// <summary>
    /// Represents a type to be registered into the container
    /// </summary>
    public sealed class TypeRegistration : ContainerRegistration
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

            this.RegistrationType = registrationType;
            this.ImplementationType = implementationType;

            this.ValidateTypeCompatibility(implementationType);
        }

        /// <summary>
        /// Implementation type i.e. MyClassThatImplementsIMyInterface
        /// </summary>
        public Type ImplementationType { get; private set; }
    }
}