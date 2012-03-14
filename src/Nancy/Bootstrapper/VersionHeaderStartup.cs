using System.Collections.Generic;

namespace Nancy.Bootstrapper
{
    public class VersionHeaderStartup : IStartup
    {
        readonly VersionHeader versionHeader;

        public VersionHeaderStartup(VersionHeader versionHeader)
        {
            this.versionHeader = versionHeader;
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task`
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return null; }
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
            if (enabled)
                versionHeader.AddToPipeline(pipelines);
        }

        public static bool enabled;
        public static void Enable()
        {
            enabled = true;
        }

        public static void Disable()
        {
            enabled = false;
        }
    }
}