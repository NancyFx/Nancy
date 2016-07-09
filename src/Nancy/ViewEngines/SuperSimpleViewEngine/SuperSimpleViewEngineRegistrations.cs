namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Performs application registrations for the SuperSimpleViewEngine.
    /// </summary>
    public class SuperSimpleViewEngineRegistrations : IRegistrations
    {
        private readonly ITypeCatalog typeCatalog;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Nancy.ViewEngines.SuperSimpleViewEngine.SuperSimpleViewEngineRegistrations"/> class.
        /// </summary>
        /// <param name="typeCatalog">Type catalog.</param>
        public SuperSimpleViewEngineRegistrations (ITypeCatalog typeCatalog)
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
                    new CollectionTypeRegistration(typeof(ISuperSimpleViewEngineMatcher), this.typeCatalog.GetTypesAssignableTo<ISuperSimpleViewEngineMatcher>())
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