namespace Nancy.ViewEngines.Razor
{
    using Bootstrapper;

    /// <summary>
    /// Default dependency registrations for the <see cref="RazorViewEngine"/> class.
    /// </summary>
    public class RazorViewEngineRegistrations : Registrations
    {
        public RazorViewEngineRegistrations()
        {
            this.RegisterWithDefault<IRazorConfiguration>(typeof(DefaultRazorConfiguration));
        }
    }
}