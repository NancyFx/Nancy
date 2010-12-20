namespace Nancy.Tests.Unit
{
    using System;
    using Cookies;
    using Xunit;

    public class NancyCookieFixture
    {
        [Fact]
        public void Should_stringify_a_simple_name_value()
        {
            new NancyCookie("leto", "worm").ToString().ShouldEqual("leto=worm");
        }

        [Fact]
        public void Should_stringify_an_expiry_to_gmt_and_stupid_format()
        {
            var date = new DateTime(2015, 10, 8, 9, 10, 11, DateTimeKind.Utc);
            new NancyCookie("leto", "worm") { Expires = date }.ToString().ShouldEqual("leto=worm; expires=Thu, 08-Oct-2015 09:10:11 GMT");
        }

        [Fact]
        public void Should_stringify_a_domain()
        {
            new NancyCookie("leto", "worm") { Domain = "google.com" }.ToString().ShouldEqual("leto=worm; domain=google.com");
        }

        [Fact]
        public void Should_stringify_a_path()
        {
            new NancyCookie("leto", "worm") { Path = "/nancy" }.ToString().ShouldEqual("leto=worm; path=/nancy");
        }

        [Fact]
        public void Should_stringify_everyting()
        {
            var date = new DateTime(2016, 11, 8, 9, 10, 11, DateTimeKind.Utc);
            new NancyCookie("paul", "blind") { Expires = date, Path = "/frank", Domain = "gmail.com" }.ToString().ShouldEqual("paul=blind; expires=Tue, 08-Nov-2016 09:10:11 GMT; domain=gmail.com; path=/frank");
        }
    }
}