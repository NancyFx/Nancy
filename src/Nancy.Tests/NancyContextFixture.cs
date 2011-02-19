namespace Nancy.Tests.Unit
{
    using System;
    using FakeItEasy;
    using Xunit;

    public class NancyContextFixture
    {
        private NancyContext context;

        public NancyContextFixture()
        {
            this.context = new NancyContext();
        }

        [Fact]
        public void Disposing_context_should_dispose_disposable_items()
        {
            var disposable = A.Fake<IDisposable>();
            this.context.Items.Add("Disposable", disposable);
            this.context.Items.Add("Test", new object());

            this.context.Dispose();

            A.CallTo(() => disposable.Dispose()).MustHaveHappened(Repeated.Once);
        }

        [Fact]
        public void Disposing_context_should_clear_items_collection()
        {
            this.context.Items.Add("Test", new object());

            this.context.Dispose();

            this.context.Items.Count.ShouldEqual(0);
        }
    }
}