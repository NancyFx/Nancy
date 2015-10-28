namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using System.Threading;
    using FakeItEasy;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Nancy.Session.InProc.InProcSessionsManagement.Cache;
    using Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class PeriodicCacheCleanerFixture
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ICancellationTokenSourceFactory fakeCancellationTokenSourceFactory;
        private readonly IPeriodicTask fakePeriodicTask;
        private readonly IPeriodicTaskFactory fakePeriodicTaskFactory;
        private readonly IInProcSessionCache fakeSessionCache;
        private readonly PeriodicCacheCleaner periodicCacheCleaner;
        private readonly InProcSessionsConfiguration validConfiguration;

        public PeriodicCacheCleanerFixture()
        {
            this.fakeSessionCache = A.Fake<IInProcSessionCache>();
            this.fakePeriodicTaskFactory = A.Fake<IPeriodicTaskFactory>();
            this.fakePeriodicTask = A.Fake<IPeriodicTask>();
            this.fakeCancellationTokenSourceFactory = A.Fake<ICancellationTokenSourceFactory>();
            this.validConfiguration = new InProcSessionsConfiguration();

            this.cancellationTokenSource = new CancellationTokenSource();
            A.CallTo(() => this.fakeCancellationTokenSourceFactory.Create())
                .Returns(this.cancellationTokenSource);
            A.CallTo(() => this.fakePeriodicTaskFactory.Create(A<Action>._))
                .Returns(this.fakePeriodicTask);

            this.periodicCacheCleaner = new PeriodicCacheCleaner(
                this.validConfiguration,
                this.fakeSessionCache,
                this.fakePeriodicTaskFactory,
                this.fakeCancellationTokenSourceFactory);
        }

        [Fact]
        public void Given_null_configuration_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicCacheCleaner(
                null,
                this.fakeSessionCache,
                this.fakePeriodicTaskFactory,
                this.fakeCancellationTokenSourceFactory));
        }

        [Fact]
        public void Given_null_session_cache_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicCacheCleaner(
                this.validConfiguration,
                null,
                this.fakePeriodicTaskFactory,
                this.fakeCancellationTokenSourceFactory));
        }

        [Fact]
        public void Given_null_periodic_task_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicCacheCleaner(
                this.validConfiguration,
                this.fakeSessionCache,
                null,
                this.fakeCancellationTokenSourceFactory));
        }

        [Fact]
        public void Given_null_cancellation_token_source_factory_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicCacheCleaner(
                this.validConfiguration,
                this.fakeSessionCache,
                this.fakePeriodicTaskFactory,
                null));
        }

        [Fact]
        public void On_creation_creates_periodic_task()
        {
            A.CallTo(() => this.fakePeriodicTaskFactory.Create(A<Action>._))
                .MustHaveHappened();
        }

        [Fact]
        public void On_start_starts_created_periodic_task_with_correct_arguments()
        {
            this.periodicCacheCleaner.Start();

            A.CallTo(() => this.fakePeriodicTask.Start(
                this.validConfiguration.CacheTrimInterval,
                A<CancellationToken>._))
                .MustHaveHappened();
        }

        [Fact]
        public void On_stop_then_cancels_task()
        {
            this.periodicCacheCleaner.Start();
            this.periodicCacheCleaner.Stop();

            Assert.True(this.cancellationTokenSource.IsCancellationRequested);
        }

        [Fact]
        public void On_stop_without_start_then_does_not_throw()
        {
            Assert.DoesNotThrow(() => this.periodicCacheCleaner.Stop());
            A.CallTo(() => this.fakeCancellationTokenSourceFactory.Create())
                .MustNotHaveHappened();
        }
    }
}