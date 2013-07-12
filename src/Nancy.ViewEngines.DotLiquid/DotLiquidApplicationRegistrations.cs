namespace Nancy.ViewEngines.DotLiquid
{
    using Nancy.Bootstrapper;
    using global::DotLiquid.NamingConventions;

    public class DotLiquidApplicationRegistrations : ApplicationRegistrations
    {
        public DotLiquidApplicationRegistrations()
        {
            this.RegisterWithDefault<INamingConvention>(typeof(RubyNamingConvention));
        }
    }
}
