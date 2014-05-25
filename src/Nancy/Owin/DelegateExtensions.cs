using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nancy.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    using MiddlewareFunc = Func<
        Func<IDictionary<string, object>, Task>, 
        Func<IDictionary<string, object>, Task>>;

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
        public static Action<MiddlewareFunc> UseNancy(this Action<MiddlewareFunc> builder, Action<NancyOptions> action)
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
        public static Action<MiddlewareFunc> UseNancy(this Action<MiddlewareFunc> builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            builder(next => new NancyOwinHost(next, nancyOptions).Invoke);

            return builder;
        }
    }
}
