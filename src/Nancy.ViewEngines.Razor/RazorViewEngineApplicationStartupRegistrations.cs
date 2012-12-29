namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;
    using Bootstrapper;

    /// <summary>
    /// Default dependency registrations for the <see cref="RazorViewEngine"/> class.
    /// </summary>
    public class RazorViewEngineApplicationRegistrations : IApplicationRegistrations
    {
        /// <summary>
        /// Gets the type registrations to register for this startup task
        /// </summary>
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return new[] { new TypeRegistration(typeof(IRazorConfiguration), typeof(DefaultRazorConfiguration)), }; }
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
    }
}