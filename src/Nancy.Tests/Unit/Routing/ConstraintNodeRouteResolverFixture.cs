namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Globalization;
    using Nancy.Helpers;
    using Nancy.Testing;
    using Xunit;

    public class ConstraintNodeRouteResolverFixture
    {
        private readonly Browser browser;

        public ConstraintNodeRouteResolverFixture()
        {
            this.browser = new Browser(with => with.Module<TestModule>());
        }

        [Fact]
        public void Should_resolve_int_constraint()
        {
            var result = this.browser.Get("/intConstraint/1");

            result.Body.AsString().ShouldEqual("IntConstraint");
        }

        [Fact]
        public void Should_not_resolve_int_constraint()
        {
            var result = this.browser.Get("/intConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_decimal_constraint()
        {
            var result = this.browser.Get("/decimalConstraint/1.1");

            result.Body.AsString().ShouldEqual("DecimalConstraint");
        }

        [Fact]
        public void Should_not_resolve_decimal_constraint()
        {
            var result = this.browser.Get("/decimalConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_guid_constraint()
        {
            var result = this.browser.Get("/guidConstraint/87f8df5d-3c08-49fd-8961-d85b0984e006");

            result.Body.AsString().ShouldEqual("GuidConstraint");
        }

        [Fact]
        public void Should_not_resolve_guid_constraint()
        {
            var result = this.browser.Get("/guidConstraint/87f8df5d-3c08-49fd-8961-d85b0984e006d");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_bool_constraint()
        {
            var result = this.browser.Get("/boolConstraint/true");

            result.Body.AsString().ShouldEqual("BoolConstraint");
        }

        [Fact]
        public void Should_not_resolve_bool_constraint()
        {
            var result = this.browser.Get("/boolConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_alpha_constraint()
        {
            var result = this.browser.Get("/alphaConstraint/foo");

            result.Body.AsString().ShouldEqual("AlphaConstraint");
        }

        [Fact]
        public void Should_not_resolve_alpha_constraint()
        {
            var result = this.browser.Get("/alphaConstraint/1");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_datetime_constraint()
        {
            var dateTime = new DateTime(2010, 1, 2, 3, 4, 5, 6);
            var urlEncodeDateTime = HttpUtility.UrlEncode(dateTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            var result = this.browser.Get("/datetimeConstraint/" + urlEncodeDateTime);

            result.Body.AsString().ShouldEqual("2010-01-02 03:04:05");
        }

        [Fact]
        public void Should_not_resolve_datetime_constraint()
        {
            var result = this.browser.Get("/DateTimeConstraint/1");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_min_constraint()
        {
            var result = this.browser.Get("/minConstraint/5");

            result.Body.AsString().ShouldEqual("MinConstraint");
        }

        [Fact]
        public void Should_not_resolve_min_constraint()
        {
            var result = this.browser.Get("/minConstraint/1");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_custom_datetime_constraint()
        {
            var result = this.browser.Get("/customDatetimeConstraint/2013-10-02");

            result.Body.AsString().ShouldEqual("CustomDateTimeConstraint");
        }

        [Fact]
        public void Should_not_resolve_custom_datetime_constraint()
        {
            var result = this.browser.Get("/customDatetimeConstraint/2013-20-02");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_not_resolve_min_constraint_as_string()
        {
            var result = this.browser.Get("/minConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_max_constraint()
        {
            var result = this.browser.Get("/maxConstraint/4");

            result.Body.AsString().ShouldEqual("MaxConstraint");
        }

        [Fact]
        public void Should_not_resolve_max_constraint()
        {
            var result = this.browser.Get("/maxConstraint/7");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_not_resolve_max_constraint_as_string()
        {
            var result = this.browser.Get("/maxConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_range_constraint()
        {
            var result = this.browser.Get("/rangeConstraint/15");

            result.Body.AsString().ShouldEqual("RangeConstraint");
        }

        [Fact]
        public void Should_not_resolve_range_constraint_as_too_low()
        {
            var result = this.browser.Get("/rangeConstraint/1");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_not_resolve_range_constraint_as_too_high()
        {
            var result = this.browser.Get("/rangeConstraint/30");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_not_resolve_range_constraint_as_string()
        {
            var result = this.browser.Get("/rangeConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_minlength_constraint()
        {
            var result = this.browser.Get("/minlengthConstraint/foobar");

            result.Body.AsString().ShouldEqual("MinLengthConstraint");
        }

        [Fact]
        public void Should_not_resolve_minlength_constraint()
        {
            var result = this.browser.Get("/minLengthConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_maxlength_constraint()
        {
            var result = this.browser.Get("/maxlengthConstraint/foobar");

            result.Body.AsString().ShouldEqual("MaxLengthConstraint");
        }

        [Fact]
        public void Should_not_resolve_maxlength_constraint()
        {
            var result = this.browser.Get("/maxlengthConstraint/foobarfoobar");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_length_constraint_max_only()
        {
            var result = this.browser.Get("/lengthMaxOnlyConstraint/foobarfoobar");

            result.Body.AsString().ShouldEqual("LengthMaxOnlyConstraint");
        }

        [Fact]
        public void Should_not_resolve_range_constraint_max_only_as_too_high()
        {
            var result = this.browser.Get("/lengthMaxOnlyConstraint/foobarfoobarfoobarfoobar");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_resolve_length_constraint()
        {
            var result = this.browser.Get("/lengthConstraint/foobar");

            result.Body.AsString().ShouldEqual("LengthConstraint");
        }

        [Fact]
        public void Should_not_resolve_length_constraint_as_too_low()
        {
            var result = this.browser.Get("/lengthConstraint/foo");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_not_resolve_length_constraint_as_too_high()
        {
            var result = this.browser.Get("/lengthConstraint/foobarfoobarfoobarfoobar");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_be_case_insensitive()
        {
            var result = this.browser.Get("/caseInsensitive/foobarfoobar");

            result.Body.AsString().ShouldEqual("CaseInsensitive");
        }
        
        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get["/intConstraint/{value:int}"] = _ => "IntConstraint";

                Get["/decimalConstraint/{value:decimal}"] = _ => "DecimalConstraint";

                Get["/guidConstraint/{value:guid}"] = _ => "GuidConstraint";

                Get["/boolConstraint/{value:bool}"] = _ => "BoolConstraint";

                Get["/alphaConstraint/{value:alpha}"] = _ => "AlphaConstraint";

                Get["/datetimeConstraint/{value:datetime}"] = _ => DateTime.Parse(_.value).ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);

                Get["/customDatetimeConstraint/{value:datetime(yyyy-MM-dd)}"] = _ => "CustomDateTimeConstraint";

                Get["/minConstraint/{value:min(4)}"] = _ => "MinConstraint";

                Get["/maxConstraint/{value:max(6)}"] = _ => "MaxConstraint";

                Get["/rangeConstraint/{value:range(10, 20)}"] = _ => "RangeConstraint";

                Get["/minlengthConstraint/{value:minlength(4)}"] = _ => "MinLengthConstraint";

                Get["/maxlengthConstraint/{value:maxlength(10)}"] = _ => "MaxLengthConstraint";

                Get["/lengthMaxOnlyConstraint/{value:length(20)}"] = _ => "LengthMaxOnlyConstraint";

                Get["/lengthConstraint/{value:length(5, 20)}"] = _ => "LengthConstraint";

                Get["/caseInsensitive/{vAlUe:LeNgTh(5, 20)}"] = _ => "CaseInsensitive";
            }
        }
    }
}
