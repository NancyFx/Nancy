namespace Nancy.Bootstrapper
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a hook to perform registrations during application startup.
    /// </summary>
    public interface IRegistrations
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        IEnumerable<TypeRegistration> TypeRegistrations { get; }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        IEnumerable<InstanceRegistration> InstanceRegistrations { get; }
    }
}