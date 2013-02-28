// ReSharper disable CheckNamespace
namespace Owin
// ReSharper restore CheckNamespace
{
    using Nancy.Bootstrapper;
    using Nancy.Owin;

    /// <summary>
    /// OWIN extensions for Nancy
    /// </summary>
    public static class Extensions
    {
        public static void UseNancy(this IAppBuilder builder, INancyBootstrapper bootstrapper = null)
        {
            builder.Use(typeof(NancyOwinHost), bootstrapper ?? NancyBootstrapperLocator.Bootstrapper);
        }
    }
}