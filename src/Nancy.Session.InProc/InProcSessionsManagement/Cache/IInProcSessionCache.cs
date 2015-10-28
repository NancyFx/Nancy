namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an object that can cache <see cref="InProcSession"/> instances.
    /// </summary>
    internal interface IInProcSessionCache : IEnumerable<InProcSession>, IDisposable
    {
        /// <summary>
        /// Gets the number of sessions that are currently held in cache.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets the session with the specified identifier from the cache.
        /// </summary>
        /// <param name="id">The identifier of the session.</param>
        /// <returns>The session with the specified identifier, or null, if none was not found.</returns>
        InProcSession Get(SessionId id);

        /// <summary>
        /// Add a new item to the cache.
        /// </summary>
        /// <param name="session">The item to add.</param>
        void Set(InProcSession session);

        /// <summary>
        /// Remove any expired sessions from this cache.
        /// </summary>
        void Trim();
    }
}