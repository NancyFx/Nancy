namespace Nancy.Tests.Unit
{
    using System;
    using System.Globalization;
    using System.Threading;

    using Nancy.Cookies;

    using Xunit;

    public class NancyCookieFixture
    {
        [Fact]
        public void Should_stringify_a_simple_name_value()
        {
            // Given
            var cookie = new NancyCookie("leto", "worm");

            // When
            var stringified = cookie.ToString();

            // Then
            stringified.ShouldEqual("leto=worm; path=/");
        }

        [Fact]
        public void Should_stringify_an_expiry_to_gmt_and_stupid_format()
        {
            // Given
            var date = new DateTime(2015, 10, 8, 9, 10, 11, DateTimeKind.Utc);

            // When
            var cookie = new NancyCookie("leto", "worm") { Expires = date }.ToString();

            // Then
            cookie.ShouldEqual("leto=worm; path=/; expires=Thu, 08-Oct-2015 09:10:11 GMT");
        }

        [Fact]
        public void Should_stringify_an_expiry_to_english()
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                // Given
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                var date = new DateTime(2015, 10, 8, 9, 10, 11, DateTimeKind.Utc);

                // When
                var cookie = new NancyCookie("leto", "worm") { Expires = date }.ToString();

                // Then
                cookie.ShouldEqual("leto=worm; path=/; expires=Thu, 08-Oct-2015 09:10:11 GMT");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Should_stringify_a_domain()
        {
            // Given
            var cookie = new NancyCookie("leto", "worm") { Domain = "google.com" };

            // When
            var stringified = cookie.ToString();

            // Then
            stringified.ShouldEqual("leto=worm; path=/; domain=google.com");
        }

        [Fact]
        public void Should_stringify_a_path()
        {
            // Given
            var cookie = new NancyCookie("leto", "worm") { Path = "/nancy" };

            // When
            var stringified = cookie.ToString();

            // Then
            stringified.ShouldEqual("leto=worm; path=/nancy");
        }

        [Fact]
        public void Should_stringify_everyting()
        {
            // Given
            var date = new DateTime(2016, 11, 8, 9, 10, 11, DateTimeKind.Utc);
            var tuesday = GetInvariantAbbreviatedWeekdayName(date);
            var november = GetInvariantAbbreviatedMonthName(date);
            var cookie = new NancyCookie("paul", "blind", true, true, date) { Path = "/frank", Domain = "gmail.com" };

            // When
            var stringified = cookie.ToString();

            // Then
            stringified.ShouldEqual(string.Format("paul=blind; path=/frank; expires={0}, 08-{1}-2016 09:10:11 GMT; domain=gmail.com; Secure; HttpOnly", tuesday, november));
        }

        [Fact]
        public void Should_not_add_secure_if_set_to_false()
        {
            var cookie = new NancyCookie("Test", "Value");

            var result = cookie.ToString();

            result.ShouldNotContain("Secure");
        }

        [Fact]
        public void Should_add_secure_if_set_to_true()
        {
            var cookie = new NancyCookie("Test", "Value", true, true, null);

            var result = cookie.ToString();

            result.ShouldContain("Secure");
        }

        [Fact]
        public void Should_not_add_http_only_if_set_to_false()
        {
            var cookie = new NancyCookie("Test", "Value", false);

            var result = cookie.ToString();

            result.ShouldNotContain("HttpOnly");
        }

        [Fact]
        public void Should_add_http_only_if_set_to_true()
        {
            var cookie = new NancyCookie("Test", "Value", true);

            var result = cookie.ToString();

            result.ShouldContain("HttpOnly");
        }

        [Fact]
        public void Should_encode_key_and_value_when_stringified()
        {
            var cookie = new NancyCookie("Key with spaces", "Value with spaces");

            var result = cookie.ToString();

            result.ShouldEqual("Key+with+spaces=Value+with+spaces; path=/");
        }

        [Fact]
        public void Should_encode_key_if_necessary()
        {
            var cookie = new NancyCookie("with spaces", "Value");

            var result = cookie.EncodedName;

            result.ShouldEqual("with+spaces");
        }

        [Fact]
        public void Should_encode_value_if_necessary()
        {
            var cookie = new NancyCookie("Test", "Value with spaces");

            var result = cookie.EncodedValue;

            result.ShouldEqual("Value+with+spaces");
        }

        public static string GetInvariantAbbreviatedMonthName(DateTime dateTime)
        {
            return CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames[dateTime.Month - 1];
        }

        public static string GetInvariantAbbreviatedWeekdayName(DateTime dateTime)
        {
            return CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedDayNames[(int)dateTime.DayOfWeek];
        }

    }
}