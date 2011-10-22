using System;

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
        public void Should_not_trim_trailing_slash_if_requesting_root()
        {
            // Given, When
            var results = this.matcher.Match("/", "/");

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_ignore_trailing_slash_on_route_path()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/foo/bar/");

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_ignore_trailing_slash_on_request_uri()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/", "/foo/bar");

            // Then
            results.IsMatch.ShouldBeTrue();
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

        [Fact]
        public void Should_allow_regex_in_route_definition_and_capture_specified_parameters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/1234", @"/(?<foo>foo)/(?<bar>\d{4})/");

            // Then
            results.IsMatch.ShouldBeTrue();
            ((string)results.Parameters["foo"]).ShouldEqual("foo");
            ((string)results.Parameters["bar"]).ShouldEqual("1234");
        }

        [Fact]
        public void Should_allow_regex_in_route_definition_and_return_negative_result_when_it_does_not_match()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", @"/foo/(?<bar>[0-9]*)");

            // Then
            results.IsMatch.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_url_decode_captured_parameters()
        {
            // Given
            var parameter = Uri.EscapeUriString("baa ram ewe{}");
            
            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/{bar}");

            //Then
            ((string)results.Parameters["bar"]).ShouldEqual(parameter);
        }

        [Fact]
        public void Should_allow_all_of_the_unreserved_rfc_1738_characters_in_the_uri()
        {
            // Given
            const string parameter = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_.!*'()";

            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/{bar}");

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual(parameter);
        }
    }
}
