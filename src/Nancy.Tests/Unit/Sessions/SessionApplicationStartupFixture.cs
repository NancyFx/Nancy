namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Session;
    using Xunit;

    public class SessionApplicationStartupFixture
    {
        private IPipelines pipelines;
        private SessionApplicationStartup startup;

        public SessionApplicationStartupFixture()
        {
            startup = new SessionApplicationStartup();
            pipelines = A.Fake<IPipelines>();
            startup.Initialize(pipelines);
        }

        [Fact]
        public void Should_add_task_to_before_pipeline()
        {
            A.CallTo(() => pipelines.BeforeRequest.AddItemToEndOfPipeline(A<Func<NancyContext, Response>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_add_task_to_end_of_pipeline()
        {
            A.CallTo(() => pipelines.AfterRequest.AddItemToEndOfPipeline(A<Action<NancyContext>>.Ignored)).MustHaveHappened();
        }
    }
}
