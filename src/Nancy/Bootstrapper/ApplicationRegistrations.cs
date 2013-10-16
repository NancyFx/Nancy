namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class for providing application registrations
    /// </summary>
    public abstract class ApplicationRegistrations : IApplicationRegistrations
    {
        private readonly IList<CollectionTypeRegistration> collectionRegistrations = new List<CollectionTypeRegistration>();
        private readonly IList<InstanceRegistration> instanceRegistrations = new List<InstanceRegistration>();
        private readonly IList<TypeRegistration> typeRegistrations = new List<TypeRegistration>();

        /// <summary>
        /// Gets the collection registrations to register for this startup task
        /// </summary>
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return this.collectionRegistrations; }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return this.instanceRegistrations; }
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return this.typeRegistrations; }
        }

        /// <summary>
        /// Scans for the implementation of <typeparamref name="TRegistration"/> and registers it.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to scan for and register as.</typeparam>
        public void Register<TRegistration>()
        {
            var implementation = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>()
                .Single();

            this.typeRegistrations.Add(new TypeRegistration(typeof(TRegistration), implementation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRegistration"></typeparam>
        public void RegisterAll<TRegistration>()
        {
            var implementations = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>();

            var registration =
                new CollectionTypeRegistration(typeof(TRegistration), implementations);

            this.collectionRegistrations.Add(registration);
        }

        /// <summary>
        /// Registers the types provided by the <paramref name="defaultImplementations"/> parameters
        /// as <typeparamref name="TRegistration"/>.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="defaultImplementations">The types to register.</param>
        public void Register<TRegistration>(IEnumerable<Type> defaultImplementations)
        {
            this.collectionRegistrations.Add(new CollectionTypeRegistration(typeof(TRegistration), defaultImplementations));
        }

        /// <summary>
        /// Registers the type provided by the <paramref name="implementation"/> parameter
        /// as <typeparamref name="TRegistration"/>.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="implementation">The <see cref="Type"/> to register as <typeparamref name="TRegistration"/>.</param>
        public void Register<TRegistration>(Type implementation)
        {
            this.typeRegistrations.Add(new TypeRegistration(typeof(TRegistration), implementation));
        }

        /// <summary>
        /// Registers an instance as <typeparamref name="TRegistration"/>.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="instance">The instance to register.</param>
        public void Register<TRegistration>(TRegistration instance)
        {
            this.instanceRegistrations.Add(new InstanceRegistration(typeof(TRegistration), instance));
        }

        /// <summary>
        /// Scans for a <see cref="Type"/> that implements <typeparamref name="TRegistration"/>. If found, then it
        /// will be used for the registration, else it will use <paramref name="defaultImplementation"/>.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="defaultImplementation">The implementation of <typeparamref name="TRegistration"/> that will be use if no other implementation can be found.</param>
        /// <remarks>
        /// When scanning, it will exclude the assembly that the <see cref="ApplicationRegistrations"/> instance is defined in and it will also ignore
        /// the type specified by <paramref name="defaultImplementation"/>.
        /// </remarks>
        public void RegisterWithDefault<TRegistration>(Type defaultImplementation)
        {
            var implementation = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>()
                .Where(type => type.Assembly != this.GetType().Assembly)
                .SingleOrDefault(type => type != defaultImplementation);

            this.typeRegistrations.Add(new TypeRegistration(typeof(TRegistration), implementation ?? defaultImplementation));
        }

        /// <summary>
        /// Scans for an implementation of <typeparamref name="TRegistration"/> and registers it if found. If no implementation could
        /// be found, it will retrieve an instance of <typeparamref name="TRegistration"/> using the provided <paramref name="defaultImplementationFactory"/>,
        /// which will be used in the registration.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="defaultImplementationFactory">Factory that provides an instance of <typeparamref name="TRegistration"/>.</param>
        /// <remarks>When scanning, it will exclude the assembly that the <see cref="ApplicationRegistrations"/> instance is defined in</remarks>
        public void RegisterWithDefault<TRegistration>(Func<TRegistration> defaultImplementationFactory)
        {
            var implementation = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>()
                .SingleOrDefault(type => type.Assembly != this.GetType().Assembly);

            if (implementation != null)
            {
                this.typeRegistrations.Add(new TypeRegistration(typeof(TRegistration), implementation));
            }
            else
            {
                this.instanceRegistrations.Add(new InstanceRegistration(typeof(TRegistration), defaultImplementationFactory.Invoke()));
            }
        }

        /// <summary>
        /// Scans for all implementations of <typeparamref name="TRegistration"/>. If no implementations could be found, then it
        /// will register the types specified by <paramref name="defaultImplementations"/>.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="defaultImplementations">The types to register if non could be located while scanning.</param>
        /// <remarks>
        /// When scanning, it will exclude the assembly that the <see cref="ApplicationRegistrations"/> instance is defined in and it will also ignore
        /// the types specified by <paramref name="defaultImplementations"/>.
        /// </remarks>
        public void RegisterWithDefault<TRegistration>(IEnumerable<Type> defaultImplementations)
        {
            var implementations = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>()
                .Where(type => type.Assembly != this.GetType().Assembly)
                .Where(type => !defaultImplementations.Contains(type))
                .ToList();

            if (!implementations.Any())
            {
                implementations = defaultImplementations.ToList();
            }

            this.collectionRegistrations.Add(new CollectionTypeRegistration(typeof(TRegistration), implementations));
        }

        /// <summary>
        /// Scans for all implementations of <typeparamref name="TRegistration"/> and registers them, followed by the
        /// types defined by the <paramref name="defaultImplementations"/> parameter.
        /// </summary>
        /// <typeparam name="TRegistration">The <see cref="Type"/> to register as.</typeparam>
        /// <param name="defaultImplementations">The types to register last.</param>
        /// <remarks>
        /// When scanning, it will exclude the assembly that the <see cref="ApplicationRegistrations"/> instance is defined in and it will also ignore
        /// the types specified by <paramref name="defaultImplementations"/>.
        /// </remarks>
        public void RegisterWithUserThenDefault<TRegistration>(IEnumerable<Type> defaultImplementations)
        {
            var implementations = AppDomainAssemblyTypeScanner
                .TypesOf<TRegistration>()
                .Where(type => type.Assembly != this.GetType().Assembly)
                .Where(type => !defaultImplementations.Contains(type))
                .ToList();

            this.collectionRegistrations.Add(new CollectionTypeRegistration(typeof(TRegistration), implementations.Union(defaultImplementations)));
        }
    }
}