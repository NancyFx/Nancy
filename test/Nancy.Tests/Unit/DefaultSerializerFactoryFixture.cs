namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Nancy.Configuration;
    using Nancy.Json;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultSerializerFactoryFixture
    {
        private DefaultSerializerFactory serializerFactory;

        public DefaultSerializerFactoryFixture()
        {
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("application/xml")]
        [InlineData("image/png")]
        public void Should_return_serializer_for_media_range(string expectedMediaRange)
        {
            // Given
            var expectedSerializer =
                new TestableSerializer(expectedMediaRange);

            var serializers = new ISerializer[]
            {
                expectedSerializer,
                new TestableSerializer("custom/foo"),
                new TestableSerializer("custom/bar"),
                new TestableSerializer("custom/baz"),
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When
            var result = this.serializerFactory.GetSerializer(expectedMediaRange);

            // Then
            result.ShouldBeSameAs(expectedSerializer);
        }

        [Fact]
        public void Should_return_null_if_no_serializer_matched_media_range()
        {
            // Given
            var serializers = new ISerializer[]
            {
                new TestableSerializer("custom/foo"),
                new TestableSerializer("custom/bar"),
                new TestableSerializer("custom/baz"),
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When
            var result = this.serializerFactory.GetSerializer("application/json");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_not_throw_exception_if_a_serializer_throws()
        {
            // Given
            var serializers = new ISerializer[]
            {
                new ExceptionThrowingSerializer()
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When, Then
            Assert.DoesNotThrow(() => this.serializerFactory.GetSerializer("application/json"));
        }

        [Fact]
        public void Should_throw_if_multiple_serializers_matched_media_range()
        {
            var serializers = new ISerializer[]
            {
                new TestableSerializer("application/json"),
                new TestableSerializer("application/json")
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When, Then
            Assert.Throws<InvalidOperationException>(() => this.serializerFactory.GetSerializer("application/json"));
        }

        [Fact]
        public void Should_not_include_default_serializers_when_counting_matches()
        {
            var serializers = new ISerializer[]
            {
                new TestableSerializer("application/json"),
                new DefaultJsonSerializer(GetTestingEnvironment())
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When, Then
            Assert.DoesNotThrow(() => this.serializerFactory.GetSerializer("application/json"));
        }

        [Fact]
        public void Should_prioritize_non_default_serializer_match()
        {
            const string expectedMediaRange = "application/json";

            var expectedSerializer =
                new TestableSerializer(expectedMediaRange);

            var serializers = new ISerializer[]
            {
                new DefaultJsonSerializer(GetTestingEnvironment()),
                expectedSerializer
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When
            var result = this.serializerFactory.GetSerializer(expectedMediaRange);

            // Then
            result.ShouldBeSameAs(expectedSerializer);
        }

        [Fact]
        public void Should_return_default_serializer_if_no_other_match_could_be_made()
        {
            // Given
            var expectedSerializer =
                new DefaultJsonSerializer(GetTestingEnvironment());

            var serializers = new ISerializer[]
            {
                expectedSerializer,
                new TestableSerializer("application/xml"),
                new TestableSerializer("application/xml"),
            };

            this.serializerFactory = new DefaultSerializerFactory(serializers);

            // When
            var result = this.serializerFactory.GetSerializer("application/json");

            // Then
            result.ShouldBeSameAs(expectedSerializer);
        }

        private static INancyEnvironment GetTestingEnvironment()
        {
            var envionment =
                new DefaultNancyEnvironment();

            envionment.AddValue(JsonConfiguration.Default);

            return envionment;
        }

        internal class ExceptionThrowingSerializer : ISerializer
        {
            public bool CanSerialize(MediaRange mediaRange)
            {
                throw new Exception();
            }

            public IEnumerable<string> Extensions { get; set; }

            public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
            {
                throw new System.NotImplementedException();
            }
        }

        internal class TestableSerializer : ISerializer
        {
            private readonly MediaRange mediaRange;

            public TestableSerializer(MediaRange mediaRange)
            {
                this.mediaRange = mediaRange;
            }

            public bool CanSerialize(MediaRange mediaRange)
            {
                return this.mediaRange.Matches(mediaRange);
            }

            public IEnumerable<string> Extensions { get; set; }

            public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}