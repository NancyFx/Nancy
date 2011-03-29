namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using Nancy.ModelBinding;
    using Fakes;
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

        [Fact]
        public void Should_see_if_a_type_converter_is_available_for_each_property_on_the_model_where_incoming_value_exists()
        {
            var typeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => typeConverter.CanConvertTo(null)).WithAnyArguments().Returns(false);
            var binder = this.GetBinder(typeConverters: new[] { typeConverter });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";

            binder.Bind(context, typeof(TestModel));

            A.CallTo(() => typeConverter.CanConvertTo(null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void Should_call_convert_on_type_converter_if_available()
        {
            var typeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => typeConverter.CanConvertTo(typeof(string))).WithAnyArguments().Returns(true);
            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments().Returns(null);
            var binder = this.GetBinder(typeConverters: new[] { typeConverter });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";

            binder.Bind(context, typeof(TestModel));

            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_convert_basic_types()
        {
            var binder = this.GetBinder();
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";
            var now = DateTime.Now;
            context.Request.Form["DateProperty"] = now.ToString();

            var result = (TestModel)binder.Bind(context, typeof(TestModel));

            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
            result.DateProperty.ShouldEqual(now);
        }

        [Fact]
        public void Should_ignore_properties_that_cannot_be_converted()
        {
            var binder = this.GetBinder();
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";
            context.Request.Form["DateProperty"] = "Broken";

            var result = (TestModel)binder.Bind(context, typeof(TestModel));

            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
            result.DateProperty.ShouldEqual(default(DateTime));
        }

        [Fact]
        public void Should_handle_basic_array_types_in_model()
        {
            var binder = this.GetBinder();
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["Strings"] = "Test,Test2,Test3"; // This is what it looks like after being pased in Request

            var result = (ArrayModel)binder.Bind(context, typeof(ArrayModel));

            result.Strings.ShouldNotBeNull();
            result.Strings.Length.ShouldEqual(3);
        }

        [Fact]
        public void Should_call_type_converter_for_array_type_if_it_exists()
        {
            throw new NotImplementedException();
        }

        private IBinder GetBinder(IEnumerable<ITypeConverter> typeConverters = null, IEnumerable<IBodyDeserializer> bodyDeserializers = null)
        {
            return new DefaultBinder(
                typeConverters ?? new ITypeConverter[] { }, bodyDeserializers ?? new IBodyDeserializer[] { });
        }

        public class TestModel
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

            public DateTime DateProperty { get; set; }
        }

        public class BrokenModel
        {
            private string broken;
            public string Broken
            {
                get { return this.broken; }
                set { throw new NotImplementedException(); }
            }
        }

        public class ArrayModel
        {
            public string[] Strings { get; set; }
        }
    }
}