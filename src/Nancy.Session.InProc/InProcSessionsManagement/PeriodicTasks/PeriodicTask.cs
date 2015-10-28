namespace Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;

    internal class PeriodicTask : IPeriodicTask
    {
        private readonly Action action;
        private readonly ITimer timer;
        private bool isDisposed;

        public PeriodicTask(Action action, ITimer timer)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }
            this.action = action;
            this.timer = timer;
            this.isDisposed = false;
        }

        public void Start(TimeSpan interval, CancellationToken cancellationToken)
        {
            this.CheckDisposed();
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentException("The interval must be greater than zero.", "interval");
            }

            this.timer.StartTimer(() =>
            {
                if (this.IsStopRequested(cancellationToken))
                {
                    this.timer.StopTimer();
                    return;
                }
                this.action();
            }, interval);
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private bool IsStopRequested(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested || this.isDisposed;
        }
    }
}