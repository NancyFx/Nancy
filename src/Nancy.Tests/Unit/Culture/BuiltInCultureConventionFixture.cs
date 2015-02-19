namespace Nancy.Tests.Unit.Culture
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;

    using Nancy.Conventions;
    using Nancy.IO;
    using Nancy.Session;

    using Xunit;
    using Xunit.Extensions;

    public class BuiltInCultureConventionFixture
    {
        [Fact]
        public void Should_return_null_if_form_not_populated()
        {
            //Given
            var context = CreateContextRequest("/");

            //When
            var culture = BuiltInCultureConventions.FormCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Fact]
        public void Should_return_culture_if_form_populated()
        {
            //Given
            var context = PopulateForm("en-GB");

            //When
            var culture = BuiltInCultureConventions.FormCulture(context);

            //Then
            culture.Name.ShouldEqual("en-GB");
        }

        [Theory]
        [InlineData("xx-xx")]
        [InlineData("123")]
        public void Should_return_null_if_form_populated_with_invalid_culture(string cultureName)
        {
            //Given
            var context = PopulateForm(cultureName);

            //When
            var culture = BuiltInCultureConventions.FormCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/__Nancy")]
        public void Should_return_null_if_first_path_parameter_not_valid_culture(string path)
        {
            //Given
            var context = CreateContextRequest(path);

            //When
            var culture = BuiltInCultureConventions.PathCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Theory]
        [InlineData("/nl", "nl")]
        [InlineData("/en-GB", "en-GB")]
        [InlineData("/en-GB/product", "en-GB")]
        public void Should_return_culture_if_first_path_parameter_valid_culture(string path, string expected)
        {
            //Given
            var context = CreateContextRequest(path);

            //When
            var culture = BuiltInCultureConventions.PathCulture(context);

            //Then
            culture.Name.ShouldEqual(expected);
        }

        [Theory]
        [InlineData("/en-GB", "/")]
        [InlineData("/en-GB/product", "/product")]
        public void Should_culture_of_request_path_if_first_path_parameter_valid_culture(string path, string expectedPath)
        {
            //Given
            var context = CreateContextRequest(path);

            //When
            var culture = BuiltInCultureConventions.PathCulture(context);

            //Then
            context.Request.Url.Path.ShouldEqual(expectedPath);
        }

        [Fact]
        public void Should_return_null_if_headers_not_populated()
        {
            //Given
            var context = CreateContextRequest("/");

            //When
            var culture = BuiltInCultureConventions.HeaderCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Theory]
        [InlineData("xx-xx")]
        [InlineData("123")]
        public void Should_return_null_if_invalid_culture_in_header(string cultureName)
        {
            //Given
            var headers =
                new Dictionary<string, IEnumerable<string>>
                {
                    { "Accept-Language", new[] { cultureName } }
                };

            var context = CreateContextRequest("/", headers);

            //When
            var culture = BuiltInCultureConventions.HeaderCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Fact]
        public void Should_use_highest_weighted_header()
        {
            //Given
            var headers =
                new Dictionary<string, IEnumerable<string>>
                {
                    { "Accept-Language", new[] { "en-GB;q=0.8", "de-DE;q=0.7", "nl;q=0.5", "es;q=0.4" } }
                };

            var context = CreateContextRequest("/", headers);

            //When
            var culture = BuiltInCultureConventions.HeaderCulture(context);

            //Then
            culture.Name.ShouldEqual("en-GB");
        }

        [Fact]
        public void Should_return_valid_culture_if_header_populated()
        {
            //Given
            var headers =
                new Dictionary<string, IEnumerable<string>>
                {
                    { "Accept-Language", new[] { "en-GB" } }
                };

            var context = CreateContextRequest("/", headers);

            //When
            var culture = BuiltInCultureConventions.HeaderCulture(context);

            //Then
            culture.Name.ShouldEqual("en-GB");
        }

        [Fact]
        public void Should_return_null_if_session_not_populated()
        {
            //Given
            var context = CreateContextRequest("/");

            //When
            var culture = BuiltInCultureConventions.SessionCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Theory]
        [InlineData("xx-xx")]
        [InlineData("123")]
        public void Should_throw_exception_if_invalid_session_value(string cultureName)
        {
            //Given
            var context = CreateContextRequest("/");

            var sessionValues = new Dictionary<string, object>
                              {
                                  { "CurrentCulture", cultureName }
                              };

            context.Request.Session = new Session(new Dictionary<string, object>(sessionValues));

            //When
            var exception =
                Record.Exception(() => BuiltInCultureConventions.SessionCulture(context));

            //Then
            exception.ShouldBeOfType<InvalidCastException>();
        }

        [Fact]
        public void Should_return_culture_if_session_populated()
        {
            //Given
            var context = CreateContextRequest("/");

            var sessionValues = new Dictionary<string, object>
                              {
                                  { "CurrentCulture", new CultureInfo("en-GB") }
                              };

            context.Request.Session = new Session(new Dictionary<string, object>(sessionValues));

            //When
            var culture = BuiltInCultureConventions.SessionCulture(context);

            //Then
            culture.Name.ShouldEqual("en-GB");
        }

        [Fact]
        public void Should_return_null_if_no_cookie_populated()
        {
            //Given
            var context = CreateContextRequest("/");

            //When
            var culture = BuiltInCultureConventions.CookieCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Theory]
        [InlineData("xx-xx")]
        [InlineData("123")]
        public void Should_return_null_if_invalid_culture_in_cookie(string cultureName)
        {
            // Given
            const string cookieName = "CurrentCulture";
            string cookieData = cultureName;

            var headers = new Dictionary<string, IEnumerable<string>>
                              {
                                  { "cookie", new[]{ string.Format("{0}={1}", cookieName, cookieData) } }
                              };

            var context = CreateContextRequest("/", headers);

            //When
            var culture = BuiltInCultureConventions.CookieCulture(context);

            //Then
            culture.ShouldBeNull();
        }

        [Fact]
        public void Should_return_culture_if_cookie_populate()
        {
            // Given
            const string cookieName = "CurrentCulture";
            const string cookieData = "en-GB";

            var headers = new Dictionary<string, IEnumerable<string>>
                              {
                                  { "cookie", new[]{ string.Format("{0}={1}", cookieName, cookieData) } }
                              };

            var context = CreateContextRequest("/", headers);

            //When
            var culture = BuiltInCultureConventions.CookieCulture(context);

            // Then
            culture.Name.ShouldEqual("en-GB");
        }

        [Fact]
        public void Should_return_culture_from_current_thread()
        {
            //Given
            var context = CreateContextRequest("/");
            var expectedCultureName = Thread.CurrentThread.CurrentCulture.Name;

            //When
            var culture = BuiltInCultureConventions.ThreadCulture(context);

            //Then
            culture.Name.ShouldEqual(expectedCultureName);
        }

        [Fact]
        public void Validation_should_return_false_if_null_culture_name()
        {
            //Given/When
            var result = BuiltInCultureConventions.IsValidCultureInfoName(null);

            //Then
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("xx-xx")]
        [InlineData("123")]
        public void Validation_should_return_false_if_invalid_culture_name(string cultureName)
        {
            //Given/When
            var result = BuiltInCultureConventions.IsValidCultureInfoName(cultureName);

            //Then
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        [InlineData("en-US")]
        [InlineData("nl")]
        [InlineData("es")]
#if !__MonoCS__
        [InlineData("iu-Latn-CA")]
#endif
        public void Validation_should_return_true_if_valid_culture_name(string cultureName)
        {
            //Given/When
            var result = BuiltInCultureConventions.IsValidCultureInfoName(cultureName);

            //Then
            result.ShouldBeTrue();
        }

        private NancyContext PopulateForm(string cultureName)
        {
            string bodyContent = string.Concat("CurrentCulture=", cultureName);
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            writer.Write(bodyContent);
            writer.Flush();
            memory.Position = 0;

            var headers =
                new Dictionary<string, IEnumerable<string>>
                {
                    { "content-type", new[] { "application/x-www-form-urlencoded" } }
                };


            var context = new NancyContext();
            context.Request = new Request("POST", new Url {Path = "/", Scheme = "http" }, RequestStream.FromStream(memory), headers);
            return context;
        }

        private NancyContext CreateContextRequest(string path, IDictionary<string, IEnumerable<string>> cultureHeaders = null)
        {
            var context = new NancyContext();
            var request = new Request("GET", new Url{ Path = path, Scheme = "http" }, null, cultureHeaders);
            context.Request = request;
            return context;
        }
    }
}
