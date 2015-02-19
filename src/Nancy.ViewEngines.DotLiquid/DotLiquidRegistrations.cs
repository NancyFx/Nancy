namespace Nancy.ViewEngines.DotLiquid
{
    using global::DotLiquid.NamingConventions;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Used to register the default naming conventions for the ViewEngine. The naming convention affects DotLiquid Drops and filters.
    /// See <a href="https://github.com/formosatek/dotliquid/wiki/DotLiquid-Drops#tips">DotLiquid's documentation</a> on the subject for more information.
    ///
    /// This can be overridden in a bootstrapper.
    /// </summary>
    public class DotLiquidRegistrations : Registrations
    {
        /// <summary>
        /// Register the <c>RubyNamingConvention</c> as the default.
        /// </summary>
        public DotLiquidRegistrations()
        {
            this.RegisterWithDefault<INamingConvention>(typeof(RubyNamingConvention));
        }
    }
}
