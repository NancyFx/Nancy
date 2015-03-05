namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Default implementation of the <see cref="IRequestTracing"/> interface.
    /// </summary>
    public class DefaultRequestTracing : IRequestTracing
    {
        private const int MaxSize = 50;

        private readonly ConcurrentLimitedCollection<RequestTraceSession> sessions = new ConcurrentLimitedCollection<RequestTraceSession>(MaxSize);

        /// <summary>
        /// Adds the <see cref="IRequestTrace"/>, of the provided, <see cref="NancyContext"/> to the trace log.
        /// </summary>
        /// <param name="sessionId">The identifier of the trace.</param>
        /// <param name="context">A <see cref="NancyContext"/> instance.</param>
        public void AddRequestDiagnosticToSession(Guid sessionId, NancyContext context)
        {
            var session = this.sessions.FirstOrDefault(s => s.Id == sessionId);

            if (session == null)
            {
                return;
            }

            session.AddRequestTrace(context.Trace);
        }

        /// <summary>
        /// Clears the trace log.
        /// </summary>
        public void Clear()
        {
            this.sessions.Clear();
        }

        /// <summary>
        /// Creates a new trace session.
        /// </summary>
        /// <returns>A <see cref="Guid"/> which represents the identifier of the new trace session.</returns>
        public Guid CreateSession()
        {
            var id = Guid.NewGuid();

            this.sessions.Add(new RequestTraceSession(id));

            return id;
        }

        // TODO - remove above method and return guid from here?

        /// <summary>
        /// Gets all the available <see cref="RequestTraceSession"/> instances.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RequestTraceSession> GetSessions()
        {
            return this.sessions;
        }

        /// <summary>
        /// Checks if the provided <paramref name="sessionId"/> is valid or not.
        /// </summary>
        /// <param name="sessionId">A <see cref="Guid"/> representing the session to check.</param>
        /// <returns><see langword="true"/> if the session is valid, otherwise <see langword="false"/>.</returns>
        public bool IsValidSessionId(Guid sessionId)
        {
            return this.sessions.Any(s => s.Id == sessionId);
        }
    }
}