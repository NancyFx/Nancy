namespace Nancy.Testing.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Tests;
    using Xunit;

    public class BrowserResponseBodyWrapperExtensionsFixture
    {
        [Fact]
        public void Should_convert_to_string()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write("This is the content");
                    writer.Flush();
                }
            });

            // When
            var result = body.AsString();

            // Then
            result.ShouldEqual("This is the content");
        }

        [Fact]
        public void Should_retain_json_casing_for_dictionary_keys()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(@"{""name"":""Nancy"",""someWebsite"":""http://nancyfx.org"",""SomeOtherWebsite"":""https://github.com/nancyfx""}");
                    writer.Flush();
                }
            });

            // When
            var result = body.DeserializeJson<Dictionary<string, object>>();

            // Then
            result.Count.ShouldEqual(3);
            result["name"].ShouldEqual("Nancy");
            result["someWebsite"].ShouldEqual("http://nancyfx.org");
            result["SomeOtherWebsite"].ShouldEqual("https://github.com/nancyfx");
        }

        [Fact]
        public void Should_not_retain_json_casing_for_model_binding()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(@"{""name"":""Nancy"",""someWebsite"":""http://nancyfx.org"",""SomeOtherWebsite"":""https://github.com/nancyfx""}");
                    writer.Flush();
                }
            });

            // When
            var result = body.DeserializeJson<TestModel>();

            // Then
            result.Name.ShouldEqual("Nancy");
            result.SomeWebsite.ShouldEqual("http://nancyfx.org");
            result.SomeOtherWebsite.ShouldEqual("https://github.com/nancyfx");
        }

        private class TestModel
        {
            public string Name { get; set; }
            public string SomeWebsite { get; set; }
            public string SomeOtherWebsite { get; set; }
        }
    }
}