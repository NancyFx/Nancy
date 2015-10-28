namespace Nancy.Session.InProc.InProcSessionsManagement
{
    using System;

    /// <summary>
    ///  Represents a unique identifier for an in-proc session.
    /// </summary>
    public class SessionId : IEquatable<Guid>
    {
        private readonly bool isNew;
        private readonly Guid value;

        /// <summary>
        /// Creates a new instance of the <see cref="SessionId"/> class.
        /// </summary>
        internal SessionId(Guid value, bool isNew)
        {
            this.value = value;
            this.isNew = isNew;
        }

        /// <summary>
        /// Gets the actual unique identifier of the session.
        /// </summary>
        public Guid Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets a value indicating whether this session identifier is for a new session.
        /// </summary>
        public bool IsNew
        {
            get { return this.isNew; }
        }

        /// <summary>
        /// Gets a value indicating whether this is an empty session identifier.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.value == Guid.Empty; }
        }

        public bool Equals(Guid other)
        {
            return this.value == other;
        }

        public static bool operator ==(SessionId x, SessionId y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if ((object)x == null || (object)y == null)
            {
                return false;
            }
            return (x.value == y.value);
        }

        public static bool operator !=(SessionId x, SessionId y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return this.isNew ? this.value + " (new)" : this.value.ToString();
        }

        public override bool Equals(object obj)
        {
            var otherSessionId = obj as SessionId;
            if (otherSessionId == null)
            {
                return false;
            }

            return this.value == otherSessionId.value;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}