namespace Nancy.Prototype.Scanning
{
    using System;
    using System.Collections.Generic;
    using Nancy.Prototype.Configuration;
    using Nancy.Prototype.Registration;

    public static class TypeCatalogExtensions
    {
        public static IEnumerable<Type> GetTypesAssignableTo<T>(this ITypeCatalog typeCatalog)
        {
            return typeCatalog.GetTypesAssignableTo(typeof(T));
        }

        public static IEnumerable<Type> GetTypesAssignableTo(this ITypeCatalog typeCatalog, Type type)
        {
            return typeCatalog.GetTypesAssignableTo(type, ScanningStrategies.All);
        }

        public static IEnumerable<Type> GetTypesAssignableTo<T>(this ITypeCatalog typeCatalog, ScanningStrategy strategy)
        {
            return typeCatalog.GetTypesAssignableTo(typeof(T), strategy);
        }

        internal static IReadOnlyCollection<TRegistration> GetRegistrations<TRegistration>(
            this ITypeCatalog typeCatalog,
            IReadOnlyCollection<IRegistrationFactory<TRegistration>> factories)
            where TRegistration : ContainerRegistration
        {
            Check.NotNull(typeCatalog, nameof(typeCatalog));
            Check.NotNull(factories, nameof(factories));

            var registrations = new List<TRegistration>(capacity: factories.Count);

            foreach (var factory in factories)
            {
                registrations.Add(factory.GetRegistration(typeCatalog));
            }

            return registrations;
        }
    }
}
