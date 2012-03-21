namespace Nancy.ViewEngines.DotLiquid
{
    using System.Collections.Generic;
    using Bootstrapper;

    /// <summary>
    /// Registers DotLiquid specific dependencies.
    /// </summary>
    public class DotLiquidViewEngineStartup : IStartup
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { yield return new TypeRegistration(typeof(IFileSystemFactory), typeof(DefaultFileSystemFactory)); }
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

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
        }
    }
}