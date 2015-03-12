namespace Nancy.Owin
{
    using System;

    using Nancy.Bootstrapper;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
        System.Threading.Tasks.Task>;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    /// <summary>
    /// OWIN extensions for the delegate-based approach.
    /// </summary>
    public static class DelegateExtensions
    {
        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder delegate.</param>
        /// <param name="nancyBootstrapper">A Nancy bootstrapper.</param>
        /// <param name="configure">A configuration builder action.</param>
        /// <returns>The application builder delegate.</returns>
        public static Action<MidFunc> UseNancy(this Action<MidFunc> builder, INancyBootstrapper nancyBootstrapper, Action<NancyOptions> configure)
        {
            if (nancyBootstrapper == null)
            {
                throw new ArgumentNullException("nancyBootstrapper");
            }
            if (configure == null)
            {
                throw new ArgumentNullException("configure");
            }

            var options = new NancyOptions(nancyBootstrapper);

            configure(options);

            return builder.UseNancy(options);
        }

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder delegate.</param>
        /// <param name="options">The Nancy options.</param>
        /// <returns>The application builder delegate.</returns>
        public static Action<MidFunc> UseNancy(this Action<MidFunc> builder, NancyOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            builder(NancyMiddleware.UseNancy(options).Invoke);

            return builder;
        }
    }
}
