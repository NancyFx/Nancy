using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nancy.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    using MiddlewareFunc = Func<
        Func<IDictionary<string, object>, Task>, 
        Func<IDictionary<string, object>, Task>>;

    public static class DelegateExtensions
    {
        public static Action<MiddlewareFunc> UseNancy(this Action<MiddlewareFunc> builder, Action<NancyOptions> action)
        {
            var options = new NancyOptions();

            action(options);

            return builder.UseNancy(options);
        }

        public static Action<MiddlewareFunc> UseNancy(this Action<MiddlewareFunc> builder, NancyOptions options = null)
        {
            var nancyOptions = options ?? new NancyOptions();

            builder(next => new NancyOwinHost(next, nancyOptions).Invoke);

            return builder;
        }
    }
}
