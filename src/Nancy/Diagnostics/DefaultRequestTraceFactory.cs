namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Default implementation of the <see cref="IRequestTraceFactory"/> interface.
    /// </summary>
    public class DefaultRequestTraceFactory : IRequestTraceFactory
    {
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

            requestTrace.TraceLog = (StaticConfiguration.DisableErrorTraces) ?
                (ITraceLog)new NullLog() :
                (ITraceLog)new DefaultTraceLog();

            return requestTrace;
        }
    }
}