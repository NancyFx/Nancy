namespace Nancy.Tests.Unit.Responses.Negotiation
{
    using System.Linq;

    using Nancy.Responses.Negotiation;

    using Xunit;

    public class MediaRangeFixture
    {
        [Fact]
        public void Should_parse_media_range_parameters()
        {
            // When
            var range = MediaRange.FromString("application/vnd.nancy;a=1;b=2");

            // Then
            range.Parameters.Keys.ElementAt(0).ShouldEqual("a");
            range.Parameters.Keys.ElementAt(1).ShouldEqual("b");
            range.Parameters.Values.ElementAt(0).ShouldEqual("1");
            range.Parameters.Values.ElementAt(1).ShouldEqual("2");
        }

        [Fact]
        public void Should_match_with_parameters_if_parameters_match()
        {
            // Given
            var range1 = MediaRange.FromString("application/vnd.nancy;a=1;b=2");
            var range2 = MediaRange.FromString("application/vnd.nancy;a=1;b=2");

            // Then
            range1.MatchesWithParameters(range2).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_match_with_parameters_if_parameters_do_not_match()
        {
            // Given
            var range1 = MediaRange.FromString("application/vnd.nancy;a=1;b=2");
            var range2 = MediaRange.FromString("application/vnd.nancy;a=1;b=2;c=3");

            // Then
            range1.MatchesWithParameters(range2).ShouldBeFalse();
        }

        [Fact]
        public void Should_match_with_parameters_if_parameters_match_in_any_order()
        {
            // Given
            var range1 = MediaRange.FromString("application/vnd.nancy;a=1;b=2");
            var range2 = MediaRange.FromString("application/vnd.nancy;b=2;a=1");

            // Then
            range1.MatchesWithParameters(range2).ShouldBeTrue();
        }

        [Fact]
        public void Should_handle_no_parameters_when_calling_tostring()
        {
            // Given
            var range = MediaRange.FromString("application/vnd.nancy");

            // Then
            range.ToString().ShouldEqual("application/vnd.nancy");
        }

        [Fact]
        public void Should_include_parameters_when_calling_tostring()
        {
            // Given
            var range = MediaRange.FromString("application/vnd.nancy;a=1;b=2");

            // Then
            range.ToString().ShouldEqual("application/vnd.nancy;a=1;b=2");
        }

        [Fact]
        public void Should_strip_whitespace_when_calling_tostring()
        {
            // Given
            var range = MediaRange.FromString("application/vnd.nancy ; a=1; b=2");

            // Then
            range.ToString().ShouldEqual("application/vnd.nancy;a=1;b=2");
        }
    }
}