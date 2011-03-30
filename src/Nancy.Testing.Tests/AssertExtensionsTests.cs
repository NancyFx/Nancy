namespace Nancy.Testing.Tests
{
    using System;
    using System.Linq;
    using HtmlAgilityPlus;
    using Nancy.Testing;
    using Xunit;

    public class AssertExtensionsTests
    {
        private readonly QueryWrapper query;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AssertExtensionsTests()
        {
            this.query =
                new SharpQuery(@"<html><head></head><body><div id='testId' class='myClass'>Test</div><div class='anotherClass'>Tes</div><span class='class'>some contents</span><span class='class'>This has contents</span></body></html>");
        }

        [Fact]
        public void Should_throw_assertexception_when_id_does_not_exist()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#notThere"].ShouldExist());

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void Should_not_throw_exception_when_id_does_exist()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#testId"].ShouldExist());

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void Should_not_throw_exception_when_id_that_should_only_exists_once_only_exists_once()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#testId"].ShouldExistOnce());

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsOnce_ReturnsSingleItemAndConnector()
        {
            // Given, When
            var result = this.query["#testId"].ShouldExistOnce();

            // Then
            Assert.IsType<AndConnector<NodeWrapper>>(result);
        }

        [Fact]
        public void ShouldExistOnce_DoesNotExist_ShouldThrowAssert()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#notHere"].ShouldExistOnce());

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsMoreThanOnce_ShouldThrowAssert()
        {
            // Given, When
            var result = Record.Exception(() => this.query["div"].ShouldExistOnce());

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_SingleElementNotThatClass_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldBeOfClass("nope"));

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_SingleElementWithThatClass_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldBeOfClass("myClass"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldBeClass_MultipleElementsOneNotThatClass_ShouldThrowAssert()
        {
            // Given
            var htmlNodes = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNodes.ShouldBeOfClass("myClass"));

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_MultipleElementsAllThatClass_ShouldNotThrowAssert()
        {
            // Given
            var htmlNodes = this.query["span"];

            // When
            var result = Record.Exception(() => htmlNodes.ShouldBeOfClass("class"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementThatContainsText_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContain("Test"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementWithTextInDifferentCase_ShouldHonorCompareType()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContain("test", StringComparison.InvariantCultureIgnoreCase));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementDoesntContainText_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContain("nope"));

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }

        [Fact]
        public void ShouldContain_MultipleElementsAllContainingText_ShouldntThrowAssert()
        {
            // Given
            var htmlNodes = this.query["span"];

            // When
            var result = Record.Exception(() => htmlNodes.ShouldContain("contents"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_MultipleElementsOneNotContainingText_ShouldThrowAssert()
        {
            // Given
            var htmlNodes = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNodes.ShouldContain("Test"));

            // Then
            Assert.IsAssignableFrom<Xunit.Sdk.AssertException>(result);
        }
    }
}