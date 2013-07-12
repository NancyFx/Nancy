namespace Nancy.ViewEngines.DotLiquid
{
    using Nancy.Bootstrapper;
    using global::DotLiquid.NamingConventions;

    /// <summary>
    /// Used to register DotLiquid's RubyNamingConvention as the default for the ViewEngine. The naming convention affect DotLiquid Drops and filters.
    /// See <a href="https://github.com/formosatek/dotliquid/wiki/DotLiquid-Drops#tips">DotLiquid's documentation</a> on the subject for more information.
    ///
    /// This can be overridden in a bootstrapper.
    /// </summary>
    public class DotLiquidApplicationRegistrations : ApplicationRegistrations
    {
        public DotLiquidApplicationRegistrations()
        {
            this.RegisterWithDefault<INamingConvention>(typeof(RubyNamingConvention));
        }
    }
}
