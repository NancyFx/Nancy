namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Nancy.ModelBinding;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class DefaultBinderFixture
    {
        [Fact]
        public void Should_throw_if_type_converters_is_null()
        {
            var result = Record.Exception(() => new DefaultBinder(null, new IBodyDeserializer[] { }));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_body_deserializers_is_null()
        {
            var result = Record.Exception(() => new DefaultBinder(new ITypeConverter[] { }, null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_call_body_deserializer_if_one_matches()
        {
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Headers.Add("Content-Type", new[] { "application/xml" });

            binder.Bind(context, this.GetType());

            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_not_call_body_deserializer_if_none_matching()
        {
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(false);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Headers.Add("Content-Type", new[] { "application/xml" });

            binder.Bind(context, this.GetType());

            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Fact]
        public void Should_pass_request_content_type_to_can_deserialize()
        {
            var deserializer = A.Fake<IBodyDeserializer>();
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Headers.Add("Content-Type", new[] { "application/xml" });

            binder.Bind(context, this.GetType());

            A.CallTo(() => deserializer.CanDeserialize("application/xml"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_object_from_deserializer_if_one_returned()
        {
            var modelObject = new object();
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments().Returns(modelObject);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Headers.Add("Content-Type", new[] { "application/xml" });

            var result = binder.Bind(context, this.GetType());

            result.ShouldBeSameAs(modelObject);
        }

        private IBinder GetBinder(IEnumerable<ITypeConverter> typeConverters = null, IEnumerable<IBodyDeserializer> bodyDeserializers = null)
        {
            return new DefaultBinder(
                typeConverters ?? new ITypeConverter[] { }, bodyDeserializers ?? new IBodyDeserializer[] { });
        }
    }
}