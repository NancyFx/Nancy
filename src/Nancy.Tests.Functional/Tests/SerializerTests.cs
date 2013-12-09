namespace Nancy.Tests.Functional.Tests
{
    using System;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;
    using Xunit;

    public class SerializerTests
    {
        private ConfigurableBootstrapper bootstrapper;
        private Browser browser;

        public SerializerTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(SerializerTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_Serialize_To_ISO8601()
        {
            //Given
            var result = browser.Get("/serializer/20131225121030", with =>
            {
                with.Accept("application/json");
            });
            //When

            //Then
            var model = result.Body.AsString();
            
            Assert.Equal("{\"CreatedOn\":\"2013-12-25T12:10:30\"}", model);
        }
    }
}
