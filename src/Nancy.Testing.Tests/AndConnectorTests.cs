namespace Nancy.Testing.Tests
{
    using Nancy.Testing;
    using Xunit;
    using Xunit.Sdk;

    public class AndConnectorTests
    {
        [Fact]
        public void Should_allow_chaining_of_asserts_and_still_pass()
        {
            const string input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";
            var document = new DocumentWrapper(input);

            document["#testId"].ShouldExist().And.ShouldBeOfClass("myClass");
        }

        [Fact]
        public void Should_allow_chaining_of_asserts_and_fail_where_appropriate()
        {
            var result = Record.Exception(
                () =>
                    {
                        const string input =
                            @"<html><head></head><body><div id='testId' class='myOtherClass'>Test</div></body></html>";
                        var document = new DocumentWrapper(input);
                        document["#testId"].ShouldExist().And.ShouldBeOfClass("myClass");
                    });

            Assert.IsType<EqualException>(result);
        }
    }
}