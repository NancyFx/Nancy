namespace Nancy.Session.InProc.InProcSessionsManagement
{
    /// <summary>
    /// Represents an object that can create in-process memory sessions.
    /// </summary>
    internal interface IInProcSessionFactory
    {
        /// <summary>
        /// Create a new instance of <see cref="InProcSession"/>, using the specified arguments.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session.</param>
        /// <param name="wrappedSession">The inner session that is wrapped by the <see cref="InProcSession"/> instance.</param>
        /// <returns>The newly created instance.</returns>
        InProcSession Create(SessionId sessionId, ISession wrappedSession);
    }
}