using Nancy.Bootstrapper;
using Owin;

namespace Nancy.Hosting.Owin.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder RunNancy(this IAppBuilder builder)
        {
            return RunNancy(builder, NancyBootstrapperLocator.Bootstrapper);
        }

        public static IAppBuilder RunNancy(this IAppBuilder builder, INancyBootstrapper bootstrapper)
        {
            var host = new NancyOwinHost(bootstrapper);
            return builder.Use<AppDelegate>(_ => host.ProcessRequest);
        }
    }
}
