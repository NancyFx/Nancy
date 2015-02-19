namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class AbsoluteUrlTests
    {
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public AbsoluteUrlTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(AbsoluteUrlTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_Return_Response_From_Full_Url_String()
        {
            //Given, When
            var result = browser.Get("http://mydomain.com/");

            //Then
            Assert.Equal("hi", result.Body.AsString());
        }

        [Fact]
        public void Should_Return_Response_From_Full_Url()
        {
            //Given
            var url = new Url {Path = "/", Scheme = "http", HostName = "mydomain.com"};

            //When
            var result = browser.Get(url);

            //Then
            Assert.Equal("hi", result.Body.AsString());
        }

        [Fact]
        public void Should_Return_QueryString_Values_From_Full_Url_String()
        {
            //Given, When
            var result = browser.Get("http://mydomain.com/querystring?myKey=myvalue");

            //Then
            Assert.Equal("myvalue", result.Body.AsString());
        }

        [Fact]
        public void Should_Return_QueryString_Values_From_Full_Url()
        {
            //Given
            var url = new Url { Path = "/querystring", Scheme = "http", HostName = "mydomain.com", Query = "?myKey=myvalue" };

            //When
            var result = browser.Get(url);

            //Then
            Assert.Equal("myvalue", result.Body.AsString());
        }
    }
}
