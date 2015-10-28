namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents an object that can perform a periodic task.
    /// </summary>
    internal interface IPeriodicTask : IDisposable
    {
        /// <summary>
        /// Start the periodic task.
        /// </summary>
        /// <param name="interval">The interval between each execution.</param>
        /// <param name="cancellationToken">The token that can be used to cancel the periodic task.</param>
        void Start(TimeSpan interval, CancellationToken cancellationToken);
    }
}