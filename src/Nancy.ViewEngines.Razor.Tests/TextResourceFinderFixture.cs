namespace Nancy.ViewEngines.Razor.Tests
{
    using System;

    using FakeItEasy;

    using Nancy.Localization;
    using Nancy.Tests;

    using Xunit;
    using Xunit.Extensions;

    public class TextResourceFinderFixture
    {
        private readonly dynamic finder;
        private readonly ITextResource textResource;
        private readonly NancyContext context;


        public TextResourceFinderFixture()
        {
            this.context = A.Dummy<NancyContext>();
            this.textResource = A.Fake<ITextResource>();
            this.finder = new TextResourceFinder(textResource, context);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("bar")]
        public void Should_return_result_of_text_resource(string text)
        {
            // Given
            A.CallTo(() => this.textResource[A<string>._, A<NancyContext>._]).Returns(text);

            // When
            var result = (string)finder.name;

            // Then
            result.ShouldEqual(text);
        }

        [Fact]
        public void Should_invoke_text_resource_with_context()
        {
            // Given
            // When
            var result = (string)finder.name;

            // Then
            A.CallTo(() => this.textResource[A<string>._, this.context]).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_text_resource_with_member_name_when_not_chained()
        {
            // Given
            // When
            var result = (string)finder.foo;

            // Then
            A.CallTo(() => this.textResource["foo", A<NancyContext>._]).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_text_resource_with_member_name_when_chained()
        {
            // Given
            // When
            var result = (string)finder.foo.bar.other;

            // Then
            A.CallTo(() => this.textResource["foo.bar.other", A<NancyContext>._]).MustHaveHappened();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_trying_to_cast_to_anything_but_string()
        {
            // Given
            // When
            var exception = Record.Exception(() => (decimal) finder.name);

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }
    }
}