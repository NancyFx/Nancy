namespace Nancy.Tests.Unit.Routing
{
    using System.Linq;

    using Nancy.Routing;

    using Xunit;

    public class DefaultRouteSegmentExtractorFixture
    {
        private readonly DefaultRouteSegmentExtractor extractor;

        public DefaultRouteSegmentExtractorFixture()
        {
            this.extractor = new DefaultRouteSegmentExtractor();
        }

        [Fact]
        public void Should_extract_segments_from_normal_path()
        {
            // Given
            const string path = "/this/is/the/segments";

            // When
            var result = this.extractor.Extract(path).ToArray();

            // Then
            result.ShouldHaveCount(4);
            result[0].ShouldEqual("this");
            result[1].ShouldEqual("is");
            result[2].ShouldEqual("the");
            result[3].ShouldEqual("segments");
        }

        [Fact]
        public void Should_extract_regex_segments()
        {
            // Given
            const string path = "/normal/(?<name>[A-Z]*)/again";

            // When
            var result = this.extractor.Extract(path).ToArray();

            // Then
            result.ShouldHaveCount(3);
            result[0].ShouldEqual("normal");
            result[1].ShouldEqual("(?<name>[A-Z]*)");
            result[2].ShouldEqual("again");
        }

        [Fact]
        public void Should_extract_regex_with_segments_that_contains_paths()
        {
            // Given
            const string path = "/normal/(?<name>/sub/path/[A-Z]*)/again";

            // When
            var result = this.extractor.Extract(path).ToArray();

            // Then
            result.ShouldHaveCount(3);
            result[0].ShouldEqual("normal");
            result[1].ShouldEqual("(?<name>/sub/path/[A-Z]*)");
            result[2].ShouldEqual("again");
        }
    }
}