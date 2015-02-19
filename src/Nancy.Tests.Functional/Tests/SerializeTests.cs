namespace Nancy.Tests.Functional.Tests
{
    using System;

    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;

    using Xunit;

    public class SerializeTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public SerializeTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(SerializeTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_return_JSON_serialized_form()
        {
            //Given
            var response = browser.Post("/serializedform", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.FormValue("SomeString", "Hi");
                with.FormValue("SomeInt", "1");
                with.FormValue("SomeBoolean", "true");
            });

            //When
            var actualModel = response.Body.DeserializeJson<EchoModel>();

            //Then
            Assert.Equal("Hi", actualModel.SomeString);
            Assert.Equal(1, actualModel.SomeInt);
            Assert.Equal(true, actualModel.SomeBoolean);
        }

        [Fact]
        public void Should_return_JSON_serialized_querystring()
        {
            //Given
            var response = browser.Get("/serializedquerystring", (with) =>
            {
                with.HttpRequest();
                with.Accept("application/json");
                with.Query("SomeString", "Hi");
                with.Query("SomeInt", "1");
                with.Query("SomeBoolean", "true");
            });

            //When
            var actualModel = response.Body.DeserializeJson<EchoModel>();

            //Then
            Assert.Equal("Hi", actualModel.SomeString);
            Assert.Equal(1, actualModel.SomeInt);
            Assert.Equal(true, actualModel.SomeBoolean);
        }

        public class EchoModel
        {
            public string SomeString { get; set; }
            public int SomeInt { get; set; }
            public bool SomeBoolean { get; set; }
        }
    }
}
