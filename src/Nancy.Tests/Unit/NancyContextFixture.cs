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
        public void Should_not_dispose_request_when_not_set()
        {
            // Given, When
            var exception = Record.Exception(() => this.context.Dispose());

            // Then
            exception.ShouldBeNull();
        }

        [Fact]
        public void Should_dispose_request_when_being_disposed()
        {
            // Given
            var request = A.Fake<Request>(x => {
                x.Implements(typeof (IDisposable));;
                x.WithArgumentsForConstructor(new[] {"GET", "/", "http"});
            });

            this.context.Request = request;

            // When
            this.context.Dispose();

            // Then
            A.CallTo(() => ((IDisposable)request).Dispose()).MustHaveHappened();
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
        public void Should_dispose_response_if_set_when_context_is_disposed()
        {
            // Given
            var response = new DisposableResponse();
            this.context.Response = response;

            // When
            this.context.Dispose();

            // Then
            response.HasBeenDisposed.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_dispose_response_when_not_set()
        {
            // Given
            this.context.Response = null;

            // When
            var exception = Record.Exception(() => this.context.Dispose());

            // Then
            exception.ShouldBeNull();
        }

        private class DisposableResponse : Response
        {
            public bool HasBeenDisposed { get; private set; }

            public override void Dispose()
            {
                this.HasBeenDisposed = true;
            }
        }
    }
}