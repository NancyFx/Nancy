namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using System.Threading;
    using Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class PeriodicTaskFixture
    {
        private readonly PeriodicTask periodicTask;
        private readonly TimerForUnitTests timer;
        private int numberOfExecutions;

        public PeriodicTaskFixture()
        {
            this.numberOfExecutions = 0;
            this.timer = new TimerForUnitTests();
            this.periodicTask = new PeriodicTask(() => this.numberOfExecutions++, this.timer);
        }

        [Fact]
        public void Given_null_action_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicTask(null, this.timer));
        }

        [Fact]
        public void Given_null_timer_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PeriodicTask(() => this.numberOfExecutions++, null));
        }

        [Fact]
        public void Given_zero_interval_time_then_throws()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentException>(() => this.periodicTask.Start(TimeSpan.Zero, tokenSource.Token));
            }
        }

        [Fact]
        public void Given_negative_interval_time_then_throws()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentException>(
                    () => this.periodicTask.Start(TimeSpan.FromSeconds(-1), tokenSource.Token));
            }
        }

        [Fact]
        public void When_cancelled_before_first_execution_then_does_not_execute()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(1000),
                    tokenSource.Token);
                this.timer.ElapseSeconds(0.1);
                tokenSource.Cancel();
                Assert.Equal(0, this.numberOfExecutions);
            }
        }

        [Fact]
        public void Adheres_interval()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(1000),
                    tokenSource.Token);
                this.timer.ElapseSeconds(2.5);
                tokenSource.Cancel();
                Assert.Equal(2, this.numberOfExecutions);
            }
        }

        [Fact]
        public void When_disposed_then_stops()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                this.periodicTask.Start(
                    TimeSpan.FromMilliseconds(1000),
                    tokenSource.Token);
                this.timer.ElapseSeconds(4.5);
                this.periodicTask.Dispose();
                this.timer.ElapseSeconds(2);
                Assert.Equal(4, this.numberOfExecutions);
            }
        }
    }
}