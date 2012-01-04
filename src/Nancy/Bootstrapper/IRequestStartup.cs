namespace Nancy.Bootstrapper
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a hook to execute code and register types during request initialisation
    /// </summary>
    public interface IRequestStartup
    {
        /// <summary>
        /// Gets the type registrations to register for this request startup task
        /// </summary>
        IEnumerable<TypeRegistration> TypeRegistrations { get; }

        /// <summary>
        /// Gets the collection registrations to register for this request startup task
        /// </summary>
        IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; }

        /// <summary>
        /// Gets the instance registrations to register for this request startup task
        /// </summary>
        IEnumerable<InstanceRegistration> InstanceRegistrations { get; }

        /// <summary>
        /// Perform any request initialisation tasks
        /// </summary>
        /// <param name="pipelines">Request pipelines</param>
        void Initialize(IPipelines pipelines);
    }
}