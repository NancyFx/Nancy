namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Nancy.Helpers;
    using Nancy.Testing;

    using Xunit;

    public class ConstraintNodeRouteResolverFixture
    {
        private readonly Browser browser;

        public ConstraintNodeRouteResolverFixture()
        {
            // Given
            this.browser = new Browser(with => with.Module<TestModule>());
        }

        [Fact]
        public async Task Should_resolve_int_constraint()
        {
            // When
            var result = await this.browser.Get("/intConstraint/1");

            // Then
            result.Body.AsString().ShouldEqual("IntConstraint");
        }

        [Fact]
        public async Task When_int_is_larger_than_max_int_should_has_no_match()
        {
            // When
            var result = await this.browser.Get("/intConstraint/" + long.MaxValue);

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            result.Body.AsString().ShouldEqual("");
        }

        [Fact]
        public async Task Should_resolve_long_constraint()
        {
            // When
            var result = await this.browser.Get("/longConstraint/" + long.MaxValue);

            // Then
            result.Body.AsString().ShouldEqual("LongConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_int_constraint()
        {
            // When
            var result = await this.browser.Get("/intConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_decimal_constraint()
        {
            // When
            var result = await this.browser.Get("/decimalConstraint/1.1");

            // Then
            result.Body.AsString().ShouldEqual("DecimalConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_decimal_constraint()
        {
            // When
            var result = await this.browser.Get("/decimalConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_guid_constraint()
        {
            // When
            var result = await this.browser.Get("/guidConstraint/87f8df5d-3c08-49fd-8961-d85b0984e006");

            // Then
            result.Body.AsString().ShouldEqual("GuidConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_guid_constraint()
        {
            // When
            var result = await this.browser.Get("/guidConstraint/87f8df5d-3c08-49fd-8961-d85b0984e006d");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_bool_constraint()
        {
            // When
            var result = await this.browser.Get("/boolConstraint/true");

            // Then
            result.Body.AsString().ShouldEqual("BoolConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_bool_constraint()
        {
            // When
            var result = await this.browser.Get("/boolConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_alpha_constraint()
        {
            // When
            var result = await this.browser.Get("/alphaConstraint/foo");

            // Then
            result.Body.AsString().ShouldEqual("AlphaConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_alpha_constraint()
        {
            // When
            var result = await this.browser.Get("/alphaConstraint/1");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_datetime_constraint()
        {
            // When
            var dateTime = new DateTime(2010, 1, 2, 3, 4, 5, 6);
            var urlEncodeDateTime = HttpUtility.UrlEncode(dateTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            var result = await this.browser.Get("/datetimeConstraint/" + urlEncodeDateTime);

            // Then
            result.Body.AsString().ShouldEqual("2010-01-02 03:04:05");
        }

        [Fact]
        public async Task Should_not_resolve_datetime_constraint()
        {
            // When
            var result = await this.browser.Get("/DateTimeConstraint/1");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_min_constraint()
        {
            // When
            var result = await this.browser.Get("/minConstraint/5");

            // Then
            result.Body.AsString().ShouldEqual("MinConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_min_constraint()
        {
            // When
            var result = await this.browser.Get("/minConstraint/1");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_custom_datetime_constraint()
        {
            // When
            var result = await this.browser.Get("/customDatetimeConstraint/2013-10-02");

            // Then
            result.Body.AsString().ShouldEqual("CustomDateTimeConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_custom_datetime_constraint()
        {
            // When
            var result = await this.browser.Get("/customDatetimeConstraint/2013-20-02");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_not_resolve_min_constraint_as_string()
        {
            // When
            var result = await this.browser.Get("/minConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_max_constraint()
        {
            // When
            var result = await this.browser.Get("/maxConstraint/4");

            // Then
            result.Body.AsString().ShouldEqual("MaxConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_max_constraint()
        {
            // When
            var result = await this.browser.Get("/maxConstraint/7");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_not_resolve_max_constraint_as_string()
        {
            // When
            var result = await this.browser.Get("/maxConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_range_constraint()
        {
            // When
            var result = await this.browser.Get("/rangeConstraint/15");

            // Then
            result.Body.AsString().ShouldEqual("RangeConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_range_constraint_as_too_low()
        {
            // When
            var result = await this.browser.Get("/rangeConstraint/1");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_not_resolve_range_constraint_as_too_high()
        {
            // When
            var result = await this.browser.Get("/rangeConstraint/30");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_not_resolve_range_constraint_as_string()
        {
            // When
            var result = await this.browser.Get("/rangeConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_minlength_constraint()
        {
            // When
            var result = await this.browser.Get("/minlengthConstraint/foobar");

            // Then
            result.Body.AsString().ShouldEqual("MinLengthConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_minlength_constraint()
        {
            // When
            var result = await this.browser.Get("/minLengthConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_maxlength_constraint()
        {
            // When
            var result = await this.browser.Get("/maxlengthConstraint/foobar");

            // Then
            result.Body.AsString().ShouldEqual("MaxLengthConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_maxlength_constraint()
        {
            // When
            var result = await this.browser.Get("/maxlengthConstraint/foobarfoobar");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_length_constraint_max_only()
        {
            // When
            var result = await this.browser.Get("/lengthMaxOnlyConstraint/foobarfoobar");

            // Then
            result.Body.AsString().ShouldEqual("LengthMaxOnlyConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_range_constraint_max_only_as_too_high()
        {
            // When
            var result = await this.browser.Get("/lengthMaxOnlyConstraint/foobarfoobarfoobarfoobar");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_resolve_length_constraint()
        {
            // When
            var result = await this.browser.Get("/lengthConstraint/foobar");

            // Then
            result.Body.AsString().ShouldEqual("LengthConstraint");
        }

        [Fact]
        public async Task Should_not_resolve_length_constraint_as_too_low()
        {
            // When
            var result = await this.browser.Get("/lengthConstraint/foo");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_not_resolve_length_constraint_as_too_high()
        {
            // When
            var result = await this.browser.Get("/lengthConstraint/foobarfoobarfoobarfoobar");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_be_case_insensitive()
        {
            // When
            var result = await this.browser.Get("/caseInsensitive/foobarfoobar");

            // Then
            result.Body.AsString().ShouldEqual("CaseInsensitive");
        }

        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get("/intConstraint/{value:int}", args => "IntConstraint");

                Get("/longConstraint/{value:long}", args => "LongConstraint");

                Get("/decimalConstraint/{value:decimal}", args => "DecimalConstraint");

                Get("/guidConstraint/{value:guid}", args => "GuidConstraint");

                Get("/boolConstraint/{value:bool}", args => "BoolConstraint");

                Get("/alphaConstraint/{value:alpha}", args => "AlphaConstraint");

                Get("/datetimeConstraint/{value:datetime}", args => DateTime.Parse(args.value).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));

                Get("/customDatetimeConstraint/{value:datetime(yyyy-MM-dd)}", args => "CustomDateTimeConstraint");

                Get("/minConstraint/{value:min(4)}", args => "MinConstraint");

                Get("/maxConstraint/{value:max(6)}", args => "MaxConstraint");

                Get("/rangeConstraint/{value:range(10, 20)}", args => "RangeConstraint");

                Get("/minlengthConstraint/{value:minlength(4)}", args => "MinLengthConstraint");

                Get("/maxlengthConstraint/{value:maxlength(10)}", args => "MaxLengthConstraint");

                Get("/lengthMaxOnlyConstraint/{value:length(20)}", args => "LengthMaxOnlyConstraint");

                Get("/lengthConstraint/{value:length(5, 20)}", args => "LengthConstraint");

                Get("/caseInsensitive/{vAlUe:LeNgTh(5, 20)}", args => "CaseInsensitive");
            }
        }
    }
}
