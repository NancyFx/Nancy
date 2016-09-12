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
        /// Initializes an instance of the <see cref="RequestTraceSession"/> class, with
        /// the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The session identifier.</param>
        public RequestTraceSession(Guid id)
        {
            this.Id = id;
            this.requestTraces = new ConcurrentLimitedCollection<IRequestTrace>(MaxSize);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The session identifier.</value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the request traces.
        /// </summary>
        /// <value>The collection of request traces.</value>
        public IEnumerable<IRequestTrace> RequestTraces
        {
            get
            {
                return this.requestTraces;
            }
        }

        /// <summary>
        /// Adds a request trace instance to the collection.
        /// </summary>
        /// <param name="trace">The trace.</param>
        public void AddRequestTrace(IRequestTrace trace)
        {
            this.requestTraces.Add(trace);
        }
    }
}