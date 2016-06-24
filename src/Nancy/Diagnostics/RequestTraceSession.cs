namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Holds trace sessions for a request.
    /// </summary>
    public class RequestTraceSession
    {
        private const int MaxSize = 500;
        private readonly ConcurrentLimitedCollection<IRequestTrace> requestTraces;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTraceSession"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public RequestTraceSession(Guid id)
        {
            this.Id = id;
            this.requestTraces = new ConcurrentLimitedCollection<IRequestTrace>(MaxSize);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the request traces.
        /// </summary>
        /// <value>
        /// The request traces.
        /// </value>
        public IEnumerable<IRequestTrace> RequestTraces
        {
            get
            {
                return this.requestTraces;
            }
        }

        /// <summary>
        /// Adds the request trace.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public void AddRequestTrace(IRequestTrace trace)
        {
            this.requestTraces.Add(trace);
        }
    }
}