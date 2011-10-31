using System.Threading.Tasks;

namespace Nancy.Tests.Unit
{
    using System;
    using FakeItEasy;
    using Xunit;

    public class NancyContextFixture
    {
        private readonly NancyContext context;

        public NancyContextFixture()
        {
            this.context = new NancyContext();
        }

        [Fact]
        public void Should_dispose_disposable_items_when_disposed()
        {
            // Given
            var disposable = A.Fake<IDisposable>();
            this.context.Items.Add("Disposable", disposable);
            this.context.Items.Add("Test", new object());

            // When
            this.context.Dispose();

            // Then
            A.CallTo(() => disposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_clear_items_collection_when_disposed()
        {
            // Given
            this.context.Items.Add("Test", new object());

            // When
            this.context.Dispose();

            // Then
            this.context.Items.Count.ShouldEqual(0);
        }

        [Fact]
        public void Should_be_able_to_access_different_thread_contexts_from_nancycontext_current_property()
        {

            // Given
            this.context.Items.Add("Test", new object());

            var currentContext = NancyContext.Current;

            NancyContext parallelCurrentContext = null;

            var parallelTask = new TaskFactory().StartNew(() =>
            {

                var nancyContext = new NancyContext();

                nancyContext.Items.Add("ParallelTest", new object());

                parallelCurrentContext = NancyContext.Current;

            });

            parallelTask.Wait();

            // Then
            Assert.NotEqual(currentContext, parallelCurrentContext);

        }

    }

}