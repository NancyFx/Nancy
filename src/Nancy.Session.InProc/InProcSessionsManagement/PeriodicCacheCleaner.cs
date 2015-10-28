namespace Nancy.Session.InProc.InProcSessionsManagement
{
    using System;
    using System.Threading;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks;

    internal class PeriodicCacheCleaner : IPeriodicCacheCleaner
    {
        private readonly ICancellationTokenSourceFactory cancellationTokenSourceFactory;
        private readonly InProcSessionsConfiguration configuration;
        private readonly IPeriodicTask periodicTask;
        private CancellationTokenSource cancellationTokenSource;

        public PeriodicCacheCleaner(
            InProcSessionsConfiguration configuration,
            IInProcSessionCache sessionCache,
            IPeriodicTaskFactory periodicTaskFactory,
            ICancellationTokenSourceFactory cancellationTokenSourceFactory)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (sessionCache == null)
            {
                throw new ArgumentNullException("sessionCache");
            }
            if (periodicTaskFactory == null)
            {
                throw new ArgumentNullException("periodicTaskFactory");
            }
            if (cancellationTokenSourceFactory == null)
            {
                throw new ArgumentNullException("cancellationTokenSourceFactory");
            }
            this.configuration = configuration;
            this.cancellationTokenSourceFactory = cancellationTokenSourceFactory;
            this.periodicTask = periodicTaskFactory.Create(sessionCache.Trim);
        }

        public void Start()
        {
            this.cancellationTokenSource = this.cancellationTokenSourceFactory.Create();
            this.periodicTask.Start(
                this.configuration.CacheTrimInterval,
                this.cancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }
        }
    }
}