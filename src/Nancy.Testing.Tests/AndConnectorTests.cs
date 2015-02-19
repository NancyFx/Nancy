namespace Nancy.Testing.Tests
{
    using System.Text;

    using Xunit;

    public class AndConnectorTests
    {
        [Fact]
        public void Should_allow_chaining_of_asserts_and_still_pass()
        {
            // Given
            const string input = @"<html><head></head><body><div id='testId' class='myClass'>Test</div></body></html>";

            var buffer =
                Encoding.UTF8.GetBytes(input);

            // When
            var document = new DocumentWrapper(buffer);

            // Then
            document["#testId"].ShouldExist().And.ShouldBeOfClass("myClass");
        }

        [Fact]
        public void Should_allow_chaining_of_asserts_and_fail_where_appropriate()
        {
            // Given
            // When
            var result = Record.Exception(
                () =>
                    {
                        const string input =
                            @"<html><head></head><body><div id='testId' class='myOtherClass'>Test</div></body></html>";

                    var buffer =
                        Encoding.UTF8.GetBytes(input);

                    var document = 
                        new DocumentWrapper(buffer);

                        document["#testId"].ShouldExist().And.ShouldBeOfClass("myClass");
                    });

            Assert.IsType<AssertException>(result);
        }
    }
}
