namespace Nancy.ViewEngines.Razor
{
    using Bootstrapper;

    /// <summary>
    /// Default dependency registrations for the <see cref="RazorViewEngine"/> class.
    /// </summary>
    public class RazorViewEngineApplicationRegistrations : ApplicationRegistrations
    {
        public RazorViewEngineApplicationRegistrations()
        {
            this.RegisterWithDefault<IRazorConfiguration>(typeof(DefaultRazorConfiguration));
        }
    }
}