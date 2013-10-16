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
    public static class Extensions
    {
        public static IAppBuilder UseNancy(this IAppBuilder builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            var appDisposing = builder.Properties["host.OnAppDisposing"] as CancellationToken?;

            if (appDisposing.HasValue)
            {
                appDisposing.Value.Register(() => nancyOptions.Bootstrapper.Dispose());
            }

            return builder.Use(typeof(NancyOwinHost), nancyOptions);
        }

        public static IAppBuilder UseNancy(this IAppBuilder builder, Action<NancyOptions> configuration)
        {
            var options = new NancyOptions();
            configuration(options);
            return UseNancy(builder, options);
        }
    }
}
