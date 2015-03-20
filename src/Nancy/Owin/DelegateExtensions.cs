namespace Nancy.Owin
{
    using System;

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
        /// <param name="action">A configuration builder action.</param>
        /// <returns>The application builder delegate.</returns>
        public static Action<MidFunc> UseNancy(this Action<MidFunc> builder, Action<NancyOptions> action)
        {
            var options = new NancyOptions();

            action(options);

            return builder.UseNancy(options);
        }

        /// <summary>
        /// Adds Nancy to the OWIN pipeline.
        /// </summary>
        /// <param name="builder">The application builder delegate.</param>
        /// <param name="options">The Nancy options.</param>
        /// <returns>The application builder delegate.</returns>
        public static Action<MidFunc> UseNancy(this Action<MidFunc> builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            builder(NancyMiddleware.UseNancy(nancyOptions).Invoke);

            return builder;
        }
    }
}
