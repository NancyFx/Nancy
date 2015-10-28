namespace Nancy.Session.InProc.InProcSessionsManagement
{
    /// <summary>
    /// Identification method for in-process memory based sessions.
    /// </summary>
    public interface IInProcSessionIdentificationMethod
    {
        /// <summary>
        /// Load the session identifier from the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The identifier of the session for the current request.</returns>
        SessionId GetCurrentSessionId(NancyContext context);

        /// <summary>
        /// Save the session identifier in the specified context.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="context">The current context.</param>
        void SaveSessionId(SessionId sessionId, NancyContext context);
    }
}