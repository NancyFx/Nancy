namespace Nancy.Tests.Unit
{
    using System;
    using System.Globalization;
    using Cookies;
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
            stringified.ShouldEqual("leto=worm");
        }

        [Fact]
        public void Should_stringify_an_expiry_to_gmt_and_stupid_format()
        {
            // Given
            var date = new DateTime(2015, 10, 8, 9, 10, 11, DateTimeKind.Utc);
            var thursday = GetLocalizedAbbreviatedWeekdayName(date);
            var october = GetLocalizedAbbreviatedMonthName(date);

            // When
            var cookie = new NancyCookie("leto", "worm") { Expires = date }.ToString();
            
            // Then
            cookie.ShouldEqual(string.Format("leto=worm; expires={0}, 08-{1}-2015 09:10:11 GMT", thursday, october));
        }

        [Fact]
        public void Should_stringify_a_domain()
        {
            // Given
            var cookie = new NancyCookie("leto", "worm") { Domain = "google.com" };

            // When
            var stringified = cookie.ToString();

            // Then
            stringified.ShouldEqual("leto=worm; domain=google.com");
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
            var tuesday = GetLocalizedAbbreviatedWeekdayName(date);
            var november = GetLocalizedAbbreviatedMonthName(date);
            var cookie = new NancyCookie("paul", "blind") { Expires = date, Path = "/frank", Domain = "gmail.com" };
                
            // When
            var stringified = cookie.ToString();
                
            // Then
            stringified.ShouldEqual(string.Format("paul=blind; expires={0}, 08-{1}-2016 09:10:11 GMT; domain=gmail.com; path=/frank", tuesday, november));
        }

        public static string GetLocalizedAbbreviatedMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[dateTime.Month - 1];
        }

        public static string GetLocalizedAbbreviatedWeekdayName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(int)dateTime.DayOfWeek];
        }

    }
}