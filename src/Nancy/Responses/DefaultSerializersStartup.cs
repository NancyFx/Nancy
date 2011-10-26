namespace Nancy.Responses
{
    using System.Collections.Generic;
    using System.Linq;
    using Bootstrapper;

    public class DefaultSerializersStartup : IStartup
    {
        public static ISerializer JsonSerializer { get; set; }
        public static ISerializer XmlSerializer { get; set; }

        public DefaultSerializersStartup(IEnumerable<ISerializer> serializers)
        {
            JsonSerializer = serializers.FirstOrDefault(s => s.CanSerialize("application/json"));
            XmlSerializer = serializers.FirstOrDefault(s => s.CanSerialize("application/xml"));
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
        }
    }
}