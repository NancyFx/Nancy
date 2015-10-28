namespace Nancy.Session.InProc.Tests
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Xunit;

    public class InProcSessionsFixture
    {
        private readonly IInProcSessionManager fakeSessionManager;

        public InProcSessionsFixture()
        {
            this.fakeSessionManager = A.Fake<IInProcSessionManager>();
        }

        [Fact]
        public void Should_add_pre_and_post_hooks_when_enabled()
        {
            var beforePipeline = new BeforePipeline();
            var afterPipeline = new AfterPipeline();
            var hooks = A.Fake<IPipelines>();
            A.CallTo(() => hooks.BeforeRequest).Returns(beforePipeline);
            A.CallTo(() => hooks.AfterRequest).Returns(afterPipeline);

            hooks.Enable(this.fakeSessionManager);

            Assert.Equal(1, beforePipeline.PipelineDelegates.Count());
            Assert.Equal(1, afterPipeline.PipelineItems.Count());
        }

        [Fact]
        public void Given_null_config_then_throws()
        {
            var hooks = A.Fake<IPipelines>();
            Assert.Throws<ArgumentNullException>(() => hooks.Enable((InProcSessionsConfiguration)null));
        }

        [Fact]
        public void Given_invalid_config_then_throws()
        {
            var hooks = A.Fake<IPipelines>();
            var invalidConfiguration = new InProcSessionsConfiguration
            {
                SessionIdentificationMethod = null,
                SessionTimeout = TimeSpan.FromSeconds(-5)
            };
            Assert.Throws<ArgumentException>(() => hooks.Enable(invalidConfiguration));
        }

        [Fact]
        public void Given_null_pipelines_then_throws()
        {
            Assert.Throws<ArgumentNullException>(() => InProcSessions.Enable(null));
        }
    }
}