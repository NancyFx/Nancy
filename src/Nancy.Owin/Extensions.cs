// ReSharper disable CheckNamespace
namespace Owin
// ReSharper restore CheckNamespace
{
    using System;

    using Nancy.Owin;

    /// <summary>
    /// OWIN extensions for Nancy
    /// </summary>
    public static class Extensions
    {
        public static IAppBuilder UseNancy(this IAppBuilder builder, NancyOptions options = null)
        {
            return builder.Use(typeof(NancyOwinHost), options ?? new NancyOptions());
        }

        public static IAppBuilder UseNancy(this IAppBuilder builder, Action<NancyOptions> configuration)
        {
            var options = new NancyOptions();
            configuration(options);
            return UseNancy(builder, options);
        }
    }
}
