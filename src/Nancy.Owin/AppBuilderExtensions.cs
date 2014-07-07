// ReSharper disable CheckNamespace
namespace Owin
// ReSharper restore CheckNamespace
{
    using System;
    using System.Threading;

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
        /// <param name="options">The Nancy options.</param>
        /// <returns>IAppBuilder.</returns>
        public static IAppBuilder UseNancy(this IAppBuilder builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            HookDisposal(builder, nancyOptions);

            return builder.Use(NancyMiddleware.UseNancy(nancyOptions));
        }

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="configuration">A configuration builder action.</param>
        /// <returns>IAppBuilder.</returns>
        public static IAppBuilder UseNancy(this IAppBuilder builder, Action<NancyOptions> configuration)
        {
            var options = new NancyOptions();
            configuration(options);
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
