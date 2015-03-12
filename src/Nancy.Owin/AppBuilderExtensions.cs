// ReSharper disable CheckNamespace
namespace Owin
// ReSharper restore CheckNamespace
{
    using System;
    using System.Threading;

    using Nancy.Bootstrapper;
    using Nancy.Owin;

    /// <summary>
    /// OWIN extensions for Nancy
    /// </summary>
    public static class AppBuilderExtensions
    {
        private const string AppDisposingKey = "host.OnAppDisposing";

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="bootstrapper">The Nancy bootstrapper.</param>
        /// <returns>IAppBuilder.</returns>
        public static IAppBuilder UseNancy(this IAppBuilder builder, INancyBootstrapper bootstrapper)
        {
            var options = new NancyOptions(bootstrapper);

            HookDisposal(builder, options);

            return builder.Use(NancyMiddleware.UseNancy(options));
        }

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="options">The Nancy options.</param>
        /// <returns>IAppBuilder.</returns>
        public static IAppBuilder UseNancy(this IAppBuilder builder, NancyOptions options)
        {
            HookDisposal(builder, options);

            return builder.Use(NancyMiddleware.UseNancy(options));
        }

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="configureOptions">A configuration builder action.</param>
        /// <returns>IAppBuilder.</returns>
        public static IAppBuilder UseNancy(this IAppBuilder builder, INancyBootstrapper bootstrapper, Action<NancyOptions> configureOptions)
        {
            var options = new NancyOptions(bootstrapper);
            configureOptions(options);
            return UseNancy(builder, options);
        }

        private static void HookDisposal(IAppBuilder builder, NancyOptions nancyOptions)
        {
            if (!builder.Properties.ContainsKey(AppDisposingKey))
            {
                return;
            }

            var appDisposing = builder.Properties[AppDisposingKey] as CancellationToken?;

            if (appDisposing.HasValue)
            {
                appDisposing.Value.Register(nancyOptions.Bootstrapper.Dispose);
            }
        }
    }
}
