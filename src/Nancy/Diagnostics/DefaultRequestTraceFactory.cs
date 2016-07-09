namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using Nancy.Configuration;

    /// <summary>
    /// Default implementation of the <see cref="IRequestTraceFactory"/> interface.
    /// </summary>
    public class DefaultRequestTraceFactory : IRequestTraceFactory
    {
        private readonly TraceConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRequestTraceFactory"/> class.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultRequestTraceFactory(INancyEnvironment environment)
        {
            this.configuration = environment.GetValue<TraceConfiguration>();
        }

        /// <summary>
        /// Creates an <see cref="IRequestTrace"/> instance.
        /// </summary>
        /// <param name="request">A <see cref="Request"/> instance.</param>
        /// <returns>An <see cref="IRequestTrace"/> instance.</returns>
        public IRequestTrace Create(Request request)
        {
            var requestTrace =
                new DefaultRequestTrace();

            var comparer = (StaticConfiguration.CaseSensitive) ?
                StringComparer.Ordinal :
                StringComparer.OrdinalIgnoreCase;

            requestTrace.Items =
                new Dictionary<string, object>(comparer);

            requestTrace.RequestData = request;

            requestTrace.TraceLog = (this.configuration.DisplayErrorTraces) ?
                (ITraceLog)new DefaultTraceLog() :
                (ITraceLog)new NullLog();

            return requestTrace;
        }
    }
}