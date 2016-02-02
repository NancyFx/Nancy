namespace Nancy.Tests.Unit.Json
{
    using System;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Json.Converters;
    using Xunit;
    using Xunit.Sdk;

    public class JavaScriptSerializerFixture
    {
        [Fact]
        public void Should_register_converters_when_asked()
        {
            // Given
            var defaultSerializer = new JavaScriptSerializer();

            // When
            var serializer = new JavaScriptSerializer(
                registerConverters: true,
                resolver: null,
                maxJsonLength: defaultSerializer.MaxJsonLength,
                recursionLimit: defaultSerializer.RecursionLimit,
                retainCasing: defaultSerializer.RetainCasing,
                iso8601DateFormat: defaultSerializer.ISO8601DateFormat,
                converters: new[] { new TestConverter() },
                primitiveConverters: new[] { new TestPrimitiveConverter() });

            var data =
                new TestData()
                {
                    ConverterData =
                        new TestConverterType()
                        {
                            Data = 42,
                        },

                    PrimitiveConverterData =
                        new TestPrimitiveConverterType()
                        {
                            Data = 1701,
                        },
                };

            const string ExpectedJSON = @"{""converterData"":{""dataValue"":42},""primitiveConverterData"":1701}";

            // Then
            serializer.Serialize(data).ShouldEqual(ExpectedJSON);

            serializer.Deserialize<TestData>(ExpectedJSON).ShouldEqual(data);
        }

        [Fact]
        public void Should_not_register_converters_when_not_asked()
        {
            // Given
            var defaultSerializer = new JavaScriptSerializer();

            // When
            var serializer = new JavaScriptSerializer(
                registerConverters: false,
                resolver: null,
                maxJsonLength: defaultSerializer.MaxJsonLength,
                recursionLimit: defaultSerializer.RecursionLimit,
                retainCasing: defaultSerializer.RetainCasing,
                iso8601DateFormat: defaultSerializer.ISO8601DateFormat,
                converters: new[] { new TestConverter() },
                primitiveConverters: new[] { new TestPrimitiveConverter() });

            var data =
                new TestData()
                {
                    ConverterData =
                        new TestConverterType()
                        {
                            Data = 42,
                        },

                    PrimitiveConverterData =
                        new TestPrimitiveConverterType()
                        {
                            Data = 1701,
                        },
                };

            const string ExpectedJSON = @"{""converterData"":{""data"":42},""primitiveConverterData"":{""data"":1701}}";

            // Then
            serializer.Serialize(data).ShouldEqual(ExpectedJSON);

            serializer.Deserialize<TestData>(ExpectedJSON).ShouldEqual(data);
        }

        [Fact]
        public void Should_use_primitive_converter_when_available()
        {
            // When
            var serializer = new JavaScriptSerializer();

            serializer.RegisterConverters(new JavaScriptPrimitiveConverter[] { new TestPrimitiveConverter() });

            // Then
            serializer.Serialize(new TestPrimitiveConverterType() { Data = 12345 }).ShouldEqual("12345");

            serializer.Deserialize<TestPrimitiveConverterType>("12345").ShouldEqual(new TestPrimitiveConverterType() { Data = 12345 });
        }

        [Fact]
        public void Should_not_use_primitive_converter_for_wrong_type()
        {
            // When
            var serializer = new JavaScriptSerializer();

            serializer.RegisterConverters(new JavaScriptPrimitiveConverter[] { new TestPrimitiveConverter() });

            // Then
            serializer.Serialize(new TestConverterType() { Data = 12345 }).ShouldEqual(@"{""data"":12345}");

            serializer.Deserialize<TestConverterType>(@"{""data"":12345}").ShouldEqual(new TestConverterType() { Data = 12345 });

            try
            {
                serializer.Deserialize<TestConverterType>("12345");
                throw new ThrowsException(typeof(InvalidCastException));
            }
            catch { }
        }

        [Fact]
        public void Should_serialize_tuples()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new TupleConverter() });

            var tuple = Tuple.Create(10, 11);
            serializer.Serialize(tuple).ShouldEqual(@"{""item1"":10,""item2"":11}");
        }

        [Fact]
        public void Should_deserialize_tuple()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new TupleConverter() });

            string body = @"{""item1"":10,""item2"":11}";
            Tuple<int, int> result = serializer.Deserialize<Tuple<int, int>>(body);
            result.ToString().ShouldEqual("(10, 11)");
        }

        [Fact]
        public void Should_deserialize_string_tuple()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new TupleConverter() });

            string body = @"{""item1"":""Hello"",""item2"":""World"",""item3"":42}";
            var result = serializer.Deserialize<Tuple<string, string, int>>(body);
            result.ToString().ShouldEqual("(Hello, World, 42)");
        }

        [Fact]
        public void Should_deserialize_type_with_tuples()
        {
            // When
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new TupleConverter() });

            // Then
            var typeWithTuple = serializer.Deserialize<TypeWithTuple>(@"{""value"":{""item1"":10,""item2"":11}}");
            typeWithTuple.Value.Item1.ShouldEqual(10);
            typeWithTuple.Value.Item2.ShouldEqual(11);
        }

        private INancyEnvironment GetTestingEnvironment(JsonConfiguration configuration)
        {
            var environment =
                new DefaultNancyEnvironment();

            environment.AddValue(configuration);

            return environment;
        }
    }
}
