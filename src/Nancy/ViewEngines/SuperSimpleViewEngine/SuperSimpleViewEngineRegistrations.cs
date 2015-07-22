namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System.Collections.Generic;
    using Bootstrapper;

    /// <summary>
    /// Performs application registations for the SuperSimpleViewEngine.
    /// </summary>
    public class SuperSimpleViewEngineRegistrations : IRegistrations
    {
        private readonly ITypeCatalog typeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperSimpleViewEngineRegistrations"/> class,
        /// using the provided <paramref name="typeCatalog"/>.
        /// </summary>
        /// <param name="typeCatalog">An <see cref="ITypeCatalog"/> instance.</param>
        public SuperSimpleViewEngineRegistrations(ITypeCatalog typeCatalog)
        {
            this.typeCatalog = typeCatalog;
        }

        /// <summary>
        /// Gets the type registrations to register for this startup task
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
            get
            {
                return new[] {
                    new CollectionTypeRegistration(typeof(ISuperSimpleViewEngineMatcher), this.typeCatalog.TypesOf<ISuperSimpleViewEngineMatcher>(ScanMode.All))
                };
            }
        }

        /// <summary>
        /// Gets the instance registrations to register for this startup task
        /// </summary>
        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }
    }
}