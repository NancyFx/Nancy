namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System;

    /// <summary>
    /// The default implementation of the SystemClock interface.
    /// </summary>
    internal class RealSystemClock : ISystemClock
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        public DateTime NowUtc
        {
            get { return DateTime.UtcNow; }
        }
    }
}