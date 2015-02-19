namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Performs application registrations for the SuperSimpleViewEngine.
    /// </summary>
    public class SuperSimpleViewEngineRegistrations : IRegistrations
    {
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
                    new CollectionTypeRegistration(typeof(ISuperSimpleViewEngineMatcher), AppDomainAssemblyTypeScanner.TypesOf<ISuperSimpleViewEngineMatcher>(ScanMode.All))
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