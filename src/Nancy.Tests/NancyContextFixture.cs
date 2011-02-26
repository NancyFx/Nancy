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
    }
}