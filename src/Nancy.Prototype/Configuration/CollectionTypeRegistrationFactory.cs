namespace Nancy.Prototype.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Prototype.Registration;
    using Nancy.Prototype.Scanning;

    internal class CollectionTypeRegistrationFactory<TService> : ICollectionTypeRegistrationFactory<TService>
    {
        private readonly IReadOnlyCollection<Type> defaultImplementationTypes;

        private readonly List<Type> implementationTypes;

        private readonly Lifetime lifetime;

        public CollectionTypeRegistrationFactory(Lifetime lifetime, IReadOnlyCollection<Type> defaultImplementationTypes)
        {
            Check.NotNull(defaultImplementationTypes, nameof(defaultImplementationTypes));

            this.lifetime = lifetime;
            this.defaultImplementationTypes = defaultImplementationTypes;
            this.implementationTypes = new List<Type>();
        }

        public void Use<TImplementation>() where TImplementation : TService
        {
            this.Use(typeof(TImplementation));
        }

        public void Use(Type implementationType)
        {
            Check.NotNull(implementationType, nameof(implementationType));

            this.implementationTypes.Add(implementationType);
        }

        public CollectionTypeRegistration GetRegistration(ITypeCatalog typeCatalog)
        {
            Check.NotNull(typeCatalog, nameof(typeCatalog));

            return GetRegistration(this.implementationTypes, this.lifetime)
                ?? ScanForCustomImplementations(typeCatalog, this.lifetime)
                    ?? GetDefaultRegistration(this.defaultImplementationTypes, this.lifetime);
        }

        private static CollectionTypeRegistration GetRegistration(IReadOnlyCollection<Type> implementationTypes, Lifetime lifetime)
        {
            if (implementationTypes.Count > 0)
            {
                return new CollectionTypeRegistration(typeof(TService), implementationTypes, lifetime);
            }

            return null;
        }

        private static CollectionTypeRegistration ScanForCustomImplementations(ITypeCatalog typeCatalog, Lifetime lifetime)
        {
            var customImplementationTypes = typeCatalog
                .GetTypesAssignableTo<TService>(ScanningStrategies.ExcludeNancy)
                .ToArray();

            if (customImplementationTypes.Length > 0)
            {
                return new CollectionTypeRegistration(typeof(TService), customImplementationTypes, lifetime);
            }

            return null;
        }

        private static CollectionTypeRegistration GetDefaultRegistration(IReadOnlyCollection<Type> defaultImplementationTypes, Lifetime lifetime)
        {
            return new CollectionTypeRegistration(typeof(TService), defaultImplementationTypes, lifetime);
        }
    }
}
