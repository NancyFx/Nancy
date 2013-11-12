namespace Nancy.Tests.Unit.Helpers
{
    using Nancy.Helpers;
    using Xunit;

    public class HttpUtilityFixture
    {
        [Fact]
        public void ParseQueryString_respects_case_insensitive_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = false;
            var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldEqual("value");
        }

        [Fact]
        public void ParseQueryString_respects_case_sensitive_setting()
        {
            // Given
            StaticConfiguration.CaseSensitive = true;
            var query = "key=value";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value");
            collection["KEY"].ShouldBeNull();
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_case_insensitive()
        {
            // Given
            StaticConfiguration.CaseSensitive = false;
            var query = "key=value&key=value&KEY=VALUE";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value,value,VALUE");
            collection["KEY"].ShouldEqual("value,value,VALUE");
        }

        [Fact]
        public void ParseQueryString_handles_duplicate_keys_when_case_sensitive()
        {
            // Given
            StaticConfiguration.CaseSensitive = true;
            var query = "key=value&key=value&KEY=VALUE";

            // When
            var collection = HttpUtility.ParseQueryString(query);

            // Then
            collection["key"].ShouldEqual("value,value");
            collection["KEY"].ShouldEqual("VALUE");
        }
    }
}