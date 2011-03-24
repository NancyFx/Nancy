namespace Nancy.Testing.Tests
{
    using System;
    using System.Linq;
    using HtmlAgilityPack;
    using HtmlAgilityPlus;
    using Nancy.Testing;
    using Xunit;
    using Xunit.Sdk;

    public class TestExtensionsTests
    {
        private QueryWrapper query;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TestExtensionsTests()
        {
            this.query =
                new SharpQuery(@"<html><head></head><body><div id='testId' class='myClass'>Test</div><div class='anotherClass'>Tes</div><span class='class'>some contents</span><span class='class'>This has contents</span></body></html>");
        }

        [Fact]
        public void ShouldExist_DoesntExist_ShouldThrowAssert()
        {
            var result = Record.Exception(
                () =>
                {
                    this.query["#notThere"].ShouldExist();
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldExist_Exists_ShouldNotThrowAssert()
        {
            var result = Record.Exception(
                () =>
                {
                    this.query["#testId"].ShouldExist();
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsOnce_ShouldNotThrowAssert()
        {
            var result = Record.Exception(
                () =>
                {
                    this.query["#testId"].ShouldExistOnce();
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsOnce_ReturnsSingleItemAndConnector()
        {
            var result = this.query["#testId"].ShouldExistOnce();

            Assert.IsType<AndConnector<NodeWrapper>>(result);
        }

        [Fact]
        public void ShouldExistOnce_DoesNotExist_ShouldThrowAssert()
        {
            var result = Record.Exception(
                () =>
                {
                    this.query["#notHere"].ShouldExistOnce();
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldExistOnce_ExistsMoreThanOnce_ShouldThrowAssert()
        {
            var result = Record.Exception(
                () =>
                {
                    this.query["div"].ShouldExistOnce();
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_SingleElementNotThatClass_ShouldThrowAssert()
        {
            var htmlNode = this.query["#testId"].First();

            var result = Record.Exception(
                () =>
                {
                    htmlNode.ShouldBeOfClass("nope");
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_SingleElementWithThatClass_ShouldNotThrowAssert()
        {
            var htmlNode = this.query["#testId"].First();

            var result = Record.Exception(
                () =>
                {
                    htmlNode.ShouldBeOfClass("myClass");
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldBeClass_MultipleElementsOneNotThatClass_ShouldThrowAssert()
        {
            var htmlNodes = this.query["div"];

            var result = Record.Exception(
                () =>
                {
                    htmlNodes.ShouldBeOfClass("myClass");
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldBeClass_MultipleElementsAllThatClass_ShouldNotThrowAssert()
        {
            var htmlNodes = this.query["span"];

            var result = Record.Exception(
                () =>
                {
                    htmlNodes.ShouldBeOfClass("class");
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementThatContainsText_ShouldNotThrowAssert()
        {
            var htmlNode = this.query["#testId"].First();

            var result = Record.Exception(
                () =>
                {
                    htmlNode.ShouldContain("Test");
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementWithTextInDifferentCase_ShouldHonorCompareType()
        {
            var htmlNode = this.query["#testId"].First();

            var result = Record.Exception(
                () =>
                {
                    htmlNode.ShouldContain("test", StringComparison.InvariantCultureIgnoreCase);
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_SingleElementDoesntContainText_ShouldThrowAssert()
        {
            var htmlNode = this.query["#testId"].First();

            var result = Record.Exception(
                () =>
                {
                    htmlNode.ShouldContain("nope");
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }

        [Fact]
        public void ShouldContain_MultipleElementsAllContainingText_ShouldntThrowAssert()
        {
            var htmlNodes = this.query["span"];

            var result = Record.Exception(
                () =>
                {
                    htmlNodes.ShouldContain("contents");
                });

            Assert.Null(result);
        }

        [Fact]
        public void ShouldContain_MultipleElementsOneNotContainingText_ShouldThrowAssert()
        {
            var htmlNodes = this.query["div"];

            var result = Record.Exception(
                () =>
                {
                    htmlNodes.ShouldContain("Test");
                });

            Assert.IsAssignableFrom<AssertException>(result);
        }
    }
}