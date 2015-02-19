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
                configuration => configuration.Modules(new Type[] {typeof (SerializerTestModule)}));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_Serialize_To_ISO8601()
        {
            //Given & When
            var result = browser.Get("/serializer/20131225121030", with =>
            {
                with.Accept("application/json");
            });

            //Then
            var model = result.Body.AsString();
            Assert.Equal(String.Format("{{\"createdOn\":\"2013-12-25T12:10:30.0000000{0}\"}}", GetTimezoneSuffix(new DateTime(2013, 12, 25, 12, 10, 30))), model);
        }


        private static string GetTimezoneSuffix(DateTime value)
        {
            string suffix;
            DateTime time = value.ToUniversalTime();
            TimeSpan localTZOffset;
            if (value >= time)
            {
                localTZOffset = value - time;
                suffix = "+";
            }
            else
            {
                localTZOffset = time - value;
                suffix = "-";
            }
            return suffix + localTZOffset.ToString("hh") + ":" + localTZOffset.ToString("mm");
        }
    }
}
