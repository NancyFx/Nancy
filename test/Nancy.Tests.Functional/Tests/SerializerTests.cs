namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Nancy.Configuration;
    using Nancy.Json;
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
                configuration =>
                {
                    configuration.Modules(new Type[] { typeof(SerializerTestModule) });
                });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public async Task Should_Serialize_To_ISO8601()
        {
            //Given & When
            var result = await browser.Get("/serializer/20131225121030", with =>
            {
                with.Accept("application/json");
            });

            //Then
            var model = result.Body.AsString();
            Assert.Equal(string.Format("{{\"createdOn\":\"2013-12-25T12:10:30.0000000{0}\",\"name\":null}}",
                GetTimezoneSuffix(new DateTime(2013, 12, 25, 12, 10, 30))), model);
        }


        [Fact]
        public async Task Should_BindTo_Existing_Instance_Using_Body_Serializer()
        {
            // Given
            var model = new FakeSerializerModel { Name = "Marsellus Wallace" };

            // When
            var result = await browser.Post("/serializer", with =>
                {
                    with.JsonBody(model);
                    with.Accept("application/json");
                });

            var resultModel = result.Body.DeserializeJson<FakeSerializerModel>();

            // Then
            Assert.Equal("Marsellus Wallace", resultModel.Name);
            Assert.Equal(new DateTime(2014, 01, 30), resultModel.CreatedOn);
        }

        [Fact]
        public async Task Should_BindTo_Existing_Instance_Using_Form()
        {
            // Given & When
            var result = await browser.Post("/serializer", with =>
                {
                    with.FormValue("Name", "Marsellus Wallace");
                    with.Accept("application/json");
                });

            var resultModel = result.Body.DeserializeJson<FakeSerializerModel>();

            // Then
            Assert.Equal("Marsellus Wallace", resultModel.Name);
            Assert.Equal(new DateTime(2014, 01, 30), resultModel.CreatedOn);
        }

        [Fact]
        public async Task Should_BindTo_Utc_by_Default()
        {
            // Given
            var model = new FakeSerializerModel() { CreatedOn = new DateTime(2016, 05, 06, 7, 0, 0) };

            //When
            var result = await browser.Post("/serializer/date", with =>
            {
                with.Body("{\"createdOn\":\"2016-05-06T09:00:00.0000000+02:00\",\"name\":null}", "application/json");
                with.Accept("application/json");
            });

            var resultModel = result.Body.DeserializeJson<FakeSerializerModel>();

            // Then
            Assert.Equal(model.CreatedOn, resultModel.CreatedOn);
        }

        [Fact]
        public async Task Should_BindTo_Local_With_GlobalConfigutation()
        {
            // Given
            var model = new FakeSerializerModel() { CreatedOn = new DateTime(2016, 05, 06, 9, 0, 0) };

            var bootstrapper = new ConfigurableBootstrapper(
                configuration =>
                {
                    configuration.Configure(
                        environment => environment.Globalization(supportedCultureNames: new[] { "en-US" }, dateTimeStyles: DateTimeStyles.AssumeLocal));
                    configuration.Modules(new Type[] { typeof(SerializerTestModule) });
                });

            var browser = new Browser(bootstrapper);

            //When
            var result = await browser.Post("/serializer/date", with =>
            {
                with.JsonBody(model);
                with.Accept("application/json");
            });

            var resultModel = result.Body.DeserializeJson<FakeSerializerModel>();

            // Then
            Assert.Equal(model.CreatedOn, resultModel.CreatedOn);
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
