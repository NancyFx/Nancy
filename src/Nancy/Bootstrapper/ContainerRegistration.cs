namespace Nancy.Bootstrapper
{
    using System;
    using System.Linq;

    using Nancy.Extensions;

    /// <summary>
    /// Base class for container registrations
    /// </summary>
    public abstract class ContainerRegistration
    {
        /// <summary>
        /// Gets the lifetime of the registration
        /// </summary>
        public Lifetime Lifetime { get; protected set; }

        /// <summary>
        /// Registration type i.e. IMyInterface
        /// </summary>
        public Type RegistrationType { get; protected set; }

        /// <summary>
        /// Checks if all implementation types are assignable from the registration type, otherwise throws an exception.
        /// </summary>
        /// <param name="types">The implementation types.</param>
        /// <exception cref="ArgumentException">One or more of the implementation types is not assignable from the registration type.</exception>
        /// <exception cref="InvalidOperationException">The <see cref="RegistrationType"/> property must be assigned before the method is invoked.</exception>
        protected void ValidateTypeCompatibility(params Type[] types)
        {
            if (this.RegistrationType == null)
            {
                throw new InvalidOperationException("The RegistrationType must be set first.");
            }

            var incompatibleTypes =
                types.Where(type => !this.RegistrationType.IsAssignableFrom(type) && !type.IsAssignableToGenericType(this.RegistrationType)).ToArray();

            if (incompatibleTypes.Any())
            {
                var incompatibleTypeNames =
                    string.Join(", ", incompatibleTypes.Select(type => type.FullName));

                var errorMessage =
                    string.Format("{0} must implement {1} inorder to be registered by {2}", incompatibleTypeNames, this.RegistrationType.FullName, this.GetType().Name);

                throw new ArgumentException(errorMessage);
            }
        }
    }
}