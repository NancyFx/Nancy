namespace Nancy.Testing.Tests
{
    using System;
    using System.Linq;

    using CsQuery;

    using Xunit;

    public class AssertExtensionsTests
    {
        private readonly QueryWrapper query;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public AssertExtensionsTests()
        {
            var document =
                CQ.Create(@"<html><head></head><body><div id='testId' class='myClass' attribute1 attribute2='value2'>Test</div><div class='anotherClass'>Tes</div><span class='class'>some contents</span><span class='class'>This has contents</span></body></html>");

            this.query =
                new QueryWrapper(document);
        }

        [Fact]
        public void Should_throw_assertexception_when_id_does_not_exist()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#notThere"].ShouldExist());

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
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
        public void Should_detect_nonexistence()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#jamesIsAwesome"].ShouldNotExist());

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
        public void ShouldExistsExactly2_Exists2_ReturnsResultAndConnector()
        {
            // Given, when
            var result = this.query[".class"].ShouldExistExactly(2);

            // Then
            Assert.IsType<AndConnector<QueryWrapper>>(result);
        }

        [Fact]
        public void ShouldExistsExactly3_Exists2_ReturnsResultAndConnector()
        {
            // When
            var result = Record.Exception(() => this.query[".class"].ShouldExistExactly(3));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldExistOnce_DoesNotExist_ShouldThrowAssert()
        {
            // Given, When
            var result = Record.Exception(() => this.query["#notHere"].ShouldExistOnce());

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsMoreThanOnce_ShouldThrowAssert()
        {
            // Given, When
            var result = Record.Exception(() => this.query["div"].ShouldExistOnce());

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_ZeroElements_ShouldThrowAssert()
        {
            // Given
            var queryWrapper = this.query["#missing"];

            // When
            var result = Record.Exception(() => queryWrapper.ShouldBeOfClass("nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_SingleElementNotThatClass_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldBeOfClass("nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
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
            Assert.IsAssignableFrom<AssertException>(result);
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
        public void AllShouldContain_ZeroElements_ShouldThrowAssert()
        {
            // Given
            var queryWrapper = this.query["#missing"];

            // When
            var result = Record.Exception(() => queryWrapper.AllShouldContain("Anything"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void AnyShouldContain_ZeroElements_ShouldThrowAssert()
        {
            // Given
            var queryWrapper = this.query["#missing"];

            // When
            var result = Record.Exception(() => queryWrapper.AnyShouldContain("Anything"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
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
            var result = Record.Exception(() => htmlNode.ShouldContain("test", StringComparison.OrdinalIgnoreCase));

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
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void AllShouldContain_MultipleElementsAllContainingText_ShouldntThrowAssert()
        {
            // Given
            var htmlNodes = this.query["span"];

            // When
            var result = Record.Exception(() => htmlNodes.AllShouldContain("contents"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void AnyShouldContain_MultipleElementsAllContainingText_ShouldntThrowAssert()
        {
            // Given
            var htmlNodes = this.query["span"];

            // When
            var result = Record.Exception(() => htmlNodes.AnyShouldContain("contents"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void AllShouldContain_MultipleElementsOneNotContainingText_ShouldThrowAssert()
        {
            // Given
            var htmlNodes = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNodes.AllShouldContain("Test"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void AnyShouldContain_MultipleElementsOneNotContainingText_ShouldntThrowAssert()
        {
            // Given
            var htmlNodes = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNodes.AnyShouldContain("Test"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContainAttribute_ZeroElements_ShouldThrowAssert()
        {
            // Given
            var queryWrapper = this.query["#missing"];

            // When
            var result = Record.Exception(() => queryWrapper.ShouldContainAttribute("nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_ZeroElementsNameAndValue_ShouldThrowAssert()
        {
            // Given
            var queryWrapper = this.query["#missing"];

            // When
            var result = Record.Exception(() => queryWrapper.ShouldContainAttribute("nope", "nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_SingleElementNotContainingAttribute_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }
        
        [Fact]
        public void ShouldContainAttribute_SingleElementNotContainingAttributeAndValue_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("nope", "nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_SingleElementContainingAttributeWithoutValueButShouldContainValue_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute1", "nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_SingleElementContainingAttributeWithDifferentValue_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute2", "nope"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_SingleElementContainingAttribute_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute1"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContainAttribute_SingleElementContainingAttributeAndValueButIngoringValue_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute2"));

            // Then
            Assert.Null(result);
        }
        
        [Fact]
        public void ShouldContainAttribute_SingleElementContainingAttributeAndValue_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["#testId"].First();

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute2", "value2"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContainAttribute_MultipleElementsOneNotContainingAttribute_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("attribute1"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_MultipleElementsOneNotContainingAttributeAndValue_ShouldThrowAssert()
        {
            // Given
            var htmlNode = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("class", "myClass"));

            // Then
            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContainAttribute_MultipleElementsContainingAttribute_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["div"];

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("class"));

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void ShouldContainAttribute_MultipleElementsContainingAttributeAndValue_ShouldNotThrowAssert()
        {
            // Given
            var htmlNode = this.query["span"];

            // When
            var result = Record.Exception(() => htmlNode.ShouldContainAttribute("class", "class"));

            // Then
            Assert.Null(result);
        }
    }
}