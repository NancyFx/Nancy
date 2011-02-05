namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class DefaultRoutePatternMatcherFixture
    {
        private readonly DefaultRoutePatternMatcher matcher;

        public DefaultRoutePatternMatcherFixture()
        {
            this.matcher = new DefaultRoutePatternMatcher();
        }

        [Fact]
        public void Should_return_match_result_when_paths_matched()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/foo/bar");

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_negative_match_result_when_paths_does_not_match()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/bar/foo");

            // Then
            results.IsMatch.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_case_insensitive_when_checking_for_match()
        {
            // Given, When
            var results = this.matcher.Match("/FoO/baR", "/fOO/bAr");

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_capture_parameters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/baz", "/foo/{bar}/{baz}");

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual("bar");
            ((string)results.Parameters["baz"]).ShouldEqual("baz");
        }

        [Fact]
        public void Should_treat_parameters_as_greedy()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/baz", "/foo/{bar}");

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual("bar/baz");
        }
    }
}