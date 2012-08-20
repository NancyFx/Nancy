namespace Nancy.Validation.FluentValidation
{
    using System.Collections.Generic;
    using Bootstrapper;

    /// <summary>
    /// Application registrations for Fluent Validation types.
    /// </summary>
    public class ApplicationRegistrations : IApplicationRegistrations
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                yield return new TypeRegistration(typeof(IModelValidator), typeof(FluentValidationValidator));
                yield return new TypeRegistration(typeof(IModelValidatorFactory), typeof(FluentValidationValidatorFactory));
            }
        }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }
    }
}