namespace Nancy
{
    using System.Collections.Generic;
    using Bootstrapper;
    using Responses;

    /// <summary>
    /// Assigns the root path of the application whom ever needs it.
    /// </summary>
    /// <remarks>This task is run at application startup.</remarks>
    public class RootPathStartup : IStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootPathStartup"/> class.
        /// </summary>
        /// <param name="rootPathProvider">An <see cref="IRootPathProvider"/> instance.</param>
        public RootPathStartup(IRootPathProvider rootPathProvider)
        {
            GenericFileResponse.SafePaths.Add(rootPathProvider.GetRootPath());
        }
        
        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations{ get { return null; } }

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get { return null; } }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get { return null; } }

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        public void Initialize(IPipelines pipelines)
        {
        }
    }
}