namespace Nancy.Validation.DataAnnotations
{
    using System.Collections.Generic;
    using Bootstrapper;

    /// <summary>
    /// Application registrations for Data Annotations validation types.
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
                yield return new TypeRegistration(typeof(IModelValidator), typeof(DataAnnotationsValidator));
                yield return new TypeRegistration(typeof(IModelValidatorFactory), typeof(DataAnnotationsValidatorFactory));
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