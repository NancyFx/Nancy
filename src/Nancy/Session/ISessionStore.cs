namespace Nancy.Session
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface to define a session storage provider
    /// </summary>
    public interface ISessionStore
    {
        /// <summary>
        /// Attempt to load a session out of the store
        /// </summary>
        /// <param name="request">The request whose session is being loaded</param>
        /// <param name="items">The loaded session or an empty (non-null) dictionary</param>
        /// <returns></returns>
        bool TryLoadSession(Request request, out IDictionary<string, object> items);
        
        /// <summary>
        /// Save a session back into the store
        /// </summary>
        /// <param name="context">The NancyContext for the current request</param>
        /// <param name="items">The new session contents. They are to be replaced in full, not merged.</param>
        void SaveSession(NancyContext context, IDictionary<string, object> items);
    }
}
