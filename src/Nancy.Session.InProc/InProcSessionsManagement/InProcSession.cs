namespace Nancy.Session.InProc.InProcSessionsManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper for the Nancy ISession interface.
    /// </summary>
    internal class InProcSession : ISession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InProcSession"/> class.
        /// </summary>
        /// <param name="id">The unique session identifier.</param>
        /// <param name="wrappedSession">The real Nancy session to wrap.</param>
        /// <param name="lastSave">The UTC time when the session was created.</param>
        /// <param name="timeout">The time after which the session should expire.</param>
        public InProcSession(SessionId id, ISession wrappedSession, DateTime lastSave, TimeSpan timeout)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (wrappedSession == null)
            {
                throw new ArgumentNullException("wrappedSession");
            }
            if (id.IsEmpty)
            {
                throw new ArgumentException("The specified session id cannot be empty", "id");
            }
            this.WrappedSession = wrappedSession;
            this.Id = id;
            this.LastSave = lastSave;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets the unique identifier of the session.
        /// </summary>
        public SessionId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The UTC time when the session was last saved.
        /// </summary>
        public DateTime LastSave
        {
            get;
            private set;
        }

        /// <summary>
        /// The time after which the session should expire.
        /// </summary>
        public TimeSpan Timeout
        {
            get;
            private set;
        }

        internal ISession WrappedSession
        {
            get;
            private set;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.WrappedSession.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The number of session values
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return this.WrappedSession.Count; }
        }

        /// <summary>
        /// Deletes the session and all associated information
        /// </summary>
        public void DeleteAll()
        {
            this.WrappedSession.DeleteAll();
        }

        /// <summary>
        /// Deletes the specific key from the session
        /// </summary>
        public void Delete(string key)
        {
            this.WrappedSession.Delete(key);
        }

        /// <summary>
        /// Retrieves the value from the session
        /// </summary>
        public object this[string key]
        {
            get { return this.WrappedSession[key]; }
            set { this.WrappedSession[key] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this session has been changed, since its creation.
        /// </summary>
        public bool HasChanged
        {
            get { return this.WrappedSession.HasChanged; }
        }

        /// <summary>
        /// Checks whether this session has expired.
        /// </summary>
        /// <param name="nowUtc">The current UTC time.</param>
        /// <returns>True if the session has expired, otherwise false.</returns>
        public bool IsExpired(DateTime nowUtc)
        {
            return nowUtc > this.LastSave.Add(this.Timeout);
        }

        public override bool Equals(object obj)
        {
            var otherSession = obj as InProcSession;
            if (otherSession == null)
            {
                return false;
            }

            return this.Id == otherSession.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}