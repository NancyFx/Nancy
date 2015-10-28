namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for request tracing.
    /// </summary>
    public interface IRequestTracing
    {
        /// <summary>
        /// Adds the <see cref="IRequestTrace"/>, of the provided, <see cref="NancyContext"/> to the trace log.
        /// </summary>
        /// <param name="sessionId">The identifier of the trace.</param>
        /// <param name="context">A <see cref="NancyContext"/> instance.</param>
        void AddRequestDiagnosticToSession(Guid sessionId, NancyContext context);

        /// <summary>
        /// Clears the trace log.
        /// </summary>
        void Clear();

        /// <summary>
        /// Creates a new trace session.
        /// </summary>
        /// <returns>A <see cref="Guid"/> which represents the identifier of the new trace session.</returns>
        Guid CreateSession();

        /// <summary>
        /// Gets all the available <see cref="RequestTraceSession"/> instances.
        /// </summary>
        /// <returns></returns>
        IEnumerable<RequestTraceSession> GetSessions();

        /// <summary>
        /// Checks if the provided <paramref name="sessionId"/> is valid or not.
        /// </summary>
        /// <param name="sessionId">A <see cref="Guid"/> representing the session to check.</param>
        /// <returns><see langword="true"/> if the session is valid, otherwise <see langword="false"/>.</returns>
        bool IsValidSessionId(Guid sessionId);
    }
}