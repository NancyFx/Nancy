namespace Nancy.Session.InProc.Tests.InProcSessionsManagement.PeriodicTasks
{
    using System;
    using Nancy.Session.InProc.InProcSessionsManagement.PeriodicTasks;
    using Xunit;

    public class PeriodicTaskFactoryFixture
    {
        private readonly PeriodicTaskFactory periodicTaskFactory;

        public PeriodicTaskFactoryFixture()
        {
            this.periodicTaskFactory = new PeriodicTaskFactory();
        }

        [Fact]
        public void Given_null_action_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => this.periodicTaskFactory.Create(null));
        }

        [Fact]
        public void Given_valid_action_then_returns_new_periodic_task()
        {
            Action action = () => Console.WriteLine("ok");
            var actual = this.periodicTaskFactory.Create(action);

            Assert.NotNull(actual);
            Assert.IsAssignableFrom<IPeriodicTask>(actual);
        }
    }
}