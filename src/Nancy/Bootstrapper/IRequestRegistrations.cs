using System.Collections.Generic;

namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Provides a hook to perform registrations during request startup.
    /// </summary>
    public interface IRequestRegistrations
    {
        /// <summary>
        /// Gets the types to register for this request startup task
        /// </summary>
        IEnumerable<TypeRegistration> TypeRegistrations { get; }

        /// <summary>
        /// Gets the collections to register for this request startup task
        /// </summary>
        IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; }

        /// <summary>
        /// Gets the instances to register for this request startup task
        /// </summary>
        IEnumerable<InstanceRegistration> InstanceRegistrations { get; }       
    }
}