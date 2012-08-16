using System;
using System.Collections.Generic;
using Nancy.Json;

namespace Nancy.Testing.Tests
{
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
        public void Should_use_converters_in_JsonSetting_when_deserializing()
        {
            // Given
            JsonSettings.Converters.Add(new OverrideConverter());
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write("{ 'Name' : 'Not overridden' }");
                    writer.Flush();
                }
            });

            // When
            var result = body.DeserializeJson<ObjectToDeserialize>();

            // Then
            result.Name.ShouldEqual("Overridden");
        }

        [Fact]
        public void Should_override_converters_in_JsonSetting_if_converters_provided_when_deserializing()
        {
            // Given
            JsonSettings.Converters.Add(new OverrideConverter());
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write("{ 'Name' : 'Not overridden' }");
                    writer.Flush();
                }
            });

            // When
            var result = body.DeserializeJson<ObjectToDeserialize>(new List<JavaScriptConverter>());

            // Then
            result.Name.ShouldEqual("Not overridden");
        }
    }

    public class ObjectToDeserialize
    {
        public string Name { get; set; }
    }

    public class OverrideConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new ObjectToDeserialize() {Name = "Overridden"};
        }


        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] { typeof(ObjectToDeserialize) }; }
        }
    }
}