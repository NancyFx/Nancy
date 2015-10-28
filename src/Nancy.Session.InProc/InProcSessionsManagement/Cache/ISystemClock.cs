namespace Nancy.Session.InProc.InProcSessionsManagement.Cache
{
    using System;

    /// <summary>
    /// Interface that represents the system clock.
    /// </summary>
    internal interface ISystemClock
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        DateTime NowUtc
        {
            get;
        }
    }
}