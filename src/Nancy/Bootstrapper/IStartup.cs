namespace Nancy.Bootstrapper
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a hook to execute code and register types during initialisation
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        IEnumerable<TypeRegistration> TypeRegistrations{ get; }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        IEnumerable<InstanceRegistration> InstanceRegistrations { get; }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        void Initialize(IPipelines pipelines);
    }
}