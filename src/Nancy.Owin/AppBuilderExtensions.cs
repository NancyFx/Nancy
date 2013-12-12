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

        public static IAppBuilder UseNancy(this IAppBuilder builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            HookDisposal(builder, nancyOptions);

            return builder.Use(typeof(NancyOwinHost), nancyOptions);
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

        public static IAppBuilder UseNancy(this IAppBuilder builder, Action<NancyOptions> configuration)
        {
            var options = new NancyOptions();
            configuration(options);
            return UseNancy(builder, options);
        }
    }
}
