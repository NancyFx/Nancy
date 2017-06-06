namespace Nancy.Prototype.Configuration
{
    using System;
    using System.Collections.Generic;
    using Nancy.Prototype.Registration;
    using Nancy.Prototype.Scanning;

    public class FrameworkConfiguration : IFrameworkConfiguration
    {
        private static readonly Type[] DefaultSerializerTypes = { };

        private readonly List<IRegistrationFactory<CollectionTypeRegistration>> collectionTypeRegistrationFactories;

        private readonly List<IRegistrationFactory<TypeRegistration>> typeRegistrationFactories;

        private readonly ITypeCatalog typeCatalog;

        public FrameworkConfiguration(ITypeCatalog typeCatalog)
        {
            Check.NotNull(typeCatalog, nameof(typeCatalog));

            this.typeCatalog = typeCatalog;

            this.typeRegistrationFactories = new List<IRegistrationFactory<TypeRegistration>>
            {
                (this.Engine = new TypeRegistrationFactory<IEngine, Engine>(Lifetime.PerRequest))
            };

            this.collectionTypeRegistrationFactories = new List<IRegistrationFactory<CollectionTypeRegistration>>
            {
                (this.Serializers = new CollectionTypeRegistrationFactory<ISerializer>(Lifetime.Singleton, DefaultSerializerTypes))
            };
        }

        public ITypeRegistrationFactory<IEngine> Engine { get; }

        public ICollectionTypeRegistrationFactory<ISerializer> Serializers { get; }

        public IContainerRegistry GetRegistry()
        {
            var typeRegistrations = this.typeCatalog.GetRegistrations(this.typeRegistrationFactories);

            var collectionTypeRegistrations = this.typeCatalog.GetRegistrations(this.collectionTypeRegistrationFactories);

            return new ContainerRegistry(typeRegistrations, collectionTypeRegistrations);
        }
    }
}
