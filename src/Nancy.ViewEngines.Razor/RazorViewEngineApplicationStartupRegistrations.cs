namespace Nancy.ViewEngines.Razor
{
    using Nancy.Bootstrapper;

    /// <summary>
    /// Default dependency registrations for the <see cref="RazorViewEngine"/> class.
    /// </summary>
    public class RazorViewEngineRegistrations : Registrations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngineRegistrations"/> class.
        /// </summary>
        /// <param name="typeCatalog">An <see cref="ITypeCatalog"/> instance.</param>
        public RazorViewEngineRegistrations(ITypeCatalog typeCatalog) : base(typeCatalog)
        {
            this.RegisterWithDefault<IRazorConfiguration>(typeof(DefaultRazorConfiguration));
        }
    }
}
