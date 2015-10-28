namespace Nancy.Session.InProc.InProcSessionsManagement
{
    /// <summary>
    /// Represents an object that loads and saves Nancy sessions.
    /// </summary>
    internal interface IInProcSessionManager
    {
        /// <summary>
        /// Save the session, for future reference.
        /// </summary>
        /// <param name="session">The session to save.</param>
        /// <param name="context">The current context.</param>
        void Save(ISession session, NancyContext context);

        /// <summary>
        /// Load the session that is owned by the specified context.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>The session that is owned by the specified context, or a new empty session, if none exists.</returns>
        ISession Load(NancyContext context);
    }
}