namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy;
    using Nancy.ModelBinding;
    using Fakes;
    using Nancy.ModelBinding.DefaultConverters;
    using Xunit;

    public class DefaultBinderFixture
    {
        private readonly IFieldNameConverter passthroughNameConverter;
        private readonly BindingDefaults emptyDefaults;

        public DefaultBinderFixture()
        {
            this.passthroughNameConverter = A.Fake<IFieldNameConverter>();
            A.CallTo(() => this.passthroughNameConverter.Convert(null)).WithAnyArguments()
                .ReturnsLazily(f => (string)f.Arguments[0]);

            this.emptyDefaults = A.Fake<BindingDefaults>();
            A.CallTo(() => this.emptyDefaults.DefaultBodyDeserializers).Returns(new IBodyDeserializer[] { });
            A.CallTo(() => this.emptyDefaults.DefaultTypeConverters).Returns(new ITypeConverter[] { });
        }

        [Fact]
        public void Should_throw_if_type_converters_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DefaultBinder(null, new IBodyDeserializer[] { }, A.Fake<IFieldNameConverter>(), new BindingDefaults()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_body_deserializers_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DefaultBinder(new ITypeConverter[] { }, null, A.Fake<IFieldNameConverter>(), new BindingDefaults()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_field_name_converter_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DefaultBinder(new ITypeConverter[] { }, new IBodyDeserializer[] { }, null, new BindingDefaults()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_defaults_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DefaultBinder(new ITypeConverter[] { }, new IBodyDeserializer[] { }, A.Fake<IFieldNameConverter>(), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_call_body_deserializer_if_one_matches()
        {
            // Given
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            binder.Bind(context, this.GetType());

            // Then
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_not_call_body_deserializer_if_doesnt_match()
        {
            // Given
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(false);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            binder.Bind(context, this.GetType());

            // Then
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Fact]
        public void Should_pass_request_content_type_to_can_deserialize()
        {
            // Then
            var deserializer = A.Fake<IBodyDeserializer>();
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            binder.Bind(context, this.GetType());

            // Then
            A.CallTo(() => deserializer.CanDeserialize("application/xml"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_object_from_deserializer_if_one_returned()
        {
            // Given
            var modelObject = new object();
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments().Returns(modelObject);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            var result = binder.Bind(context, this.GetType());

            // Then
            result.ShouldBeSameAs(modelObject);
        }

        [Fact]
        public void Should_see_if_a_type_converter_is_available_for_each_property_on_the_model_where_incoming_value_exists()
        {
            // Given
            var typeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => typeConverter.CanConvertTo(null, null)).WithAnyArguments().Returns(false);
            var binder = this.GetBinder(typeConverters: new[] { typeConverter });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";

            // When
            binder.Bind(context, typeof(TestModel));

            // Then
            A.CallTo(() => typeConverter.CanConvertTo(null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void Should_call_convert_on_type_converter_if_available()
        {
            // Given
            var typeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => typeConverter.CanConvertTo(typeof(string), null)).WithAnyArguments().Returns(true);
            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments().Returns(null);
            var binder = this.GetBinder(typeConverters: new[] { typeConverter });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";

            // When
            binder.Bind(context, typeof(TestModel));

            // Then
            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_ignore_properties_that_cannot_be_converted()
        {
            // Given
            var binder = this.GetBinder(typeConverters: new[] { new FallbackConverter() });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";
            context.Request.Form["DateProperty"] = "Broken";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
            result.DateProperty.ShouldEqual(default(DateTime));
        }

        [Fact]
        public void Should_use_field_name_converter_for_each_field()
        {
            // Given
            var binder = this.GetBinder();
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";

            // When
            binder.Bind(context, typeof(TestModel));

            // Then
            A.CallTo(() => this.passthroughNameConverter.Convert(null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void Should_not_bind_anything_on_blacklist()
        {
            // Given
            var binder = this.GetBinder(typeConverters: new[] { new FallbackConverter() });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "12";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), "IntProperty");

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Should_use_default_body_deserializer_if_one_found()
        {
            // Given
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => this.emptyDefaults.DefaultBodyDeserializers).Returns(new[] { deserializer });
            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            binder.Bind(context, this.GetType());

            // Then
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_use_default_type_converter_if_one_found()
        {
            // Given
            var typeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => typeConverter.CanConvertTo(typeof(string), null)).WithAnyArguments().Returns(true);
            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments().Returns(null);
            A.CallTo(() => this.emptyDefaults.DefaultTypeConverters).Returns(new[] { typeConverter });
            var binder = this.GetBinder();
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";

            // When
            binder.Bind(context, typeof(TestModel));

            // Then
            A.CallTo(() => typeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void User_body_serializer_should_take_precedence_over_default_one()
        {
            // Given
            var userDeserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => userDeserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            
            var defaultDeserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => defaultDeserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);

            A.CallTo(() => this.emptyDefaults.DefaultBodyDeserializers).Returns(new[] { defaultDeserializer });
            var binder = this.GetBinder(bodyDeserializers: new[] { userDeserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            binder.Bind(context, this.GetType());

            // Then
            A.CallTo(() => userDeserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => defaultDeserializer.Deserialize(null, null, null)).WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Fact]
        public void User_type_converter_should_take_precedence_over_default_one()
        {
            // Given
            var userTypeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => userTypeConverter.CanConvertTo(typeof(string), null)).WithAnyArguments().Returns(true);
            A.CallTo(() => userTypeConverter.Convert(null, null, null)).WithAnyArguments().Returns(null);
            var defaultTypeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => defaultTypeConverter.CanConvertTo(typeof(string), null)).WithAnyArguments().Returns(true);
            A.CallTo(() => defaultTypeConverter.Convert(null, null, null)).WithAnyArguments().Returns(null);
            A.CallTo(() => this.emptyDefaults.DefaultTypeConverters).Returns(new[] { defaultTypeConverter });
            var binder = this.GetBinder(new[] { userTypeConverter });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";

            // When
            binder.Bind(context, typeof(TestModel));

            // Then
            A.CallTo(() => userTypeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => defaultTypeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustNotHaveHappened();
        }

        [Fact]
        public void Should_bind_model_from_request()
        {
           
            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Query["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "0";


            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Should_bind_model_from_context_parameters()
        {
            
            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Parameters["StringProperty"] = "Test";
            context.Parameters["IntProperty"] = "0";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Form_properties_should_take_precendence_over_request_properties()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "0";
            context.Request.Query["StringProperty"] = "Test2";
            context.Request.Query["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Form_properties_should_take_precendence_over_request_properties_and_context_properties()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "0";
            context.Request.Query["StringProperty"] = "Test2";
            context.Request.Query["IntProperty"] = "1";
            context.Parameters["StringProperty"] = "Test3";
            context.Parameters["IntProperty"] = "2";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Request_properties_should_take_precendence_over_context_properties()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Request.Query["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "0";
            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Should_be_able_to_bind_from_form_and_request_simultaneously()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "0";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        [Fact]
        public void Should_be_able_to_bind_from_request_and_context_simultaneously()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Query["StringProperty"] = "Test";
            context.Parameters["IntProperty"] = "0";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel));

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(0);
        }

        private IBinder GetBinder(IEnumerable<ITypeConverter> typeConverters = null, IEnumerable<IBodyDeserializer> bodyDeserializers = null, IFieldNameConverter nameConverter = null, BindingDefaults bindingDefaults = null)
        {
            var converters = typeConverters ?? new ITypeConverter[] { };
            var deserializers = bodyDeserializers ?? new IBodyDeserializer[] { };
            var converter = nameConverter ?? this.passthroughNameConverter;
            var defaults = bindingDefaults ?? this.emptyDefaults;

            return new DefaultBinder(converters, deserializers, converter, defaults);
        }

        private static NancyContext CreateContextWithHeader(string name, IEnumerable<string> values)
        {
            var header = new Dictionary<string, IEnumerable<string>>
            {
                { name, values }
            };

            return new NancyContext()
            {
                Request = new FakeRequest("GET", "/", header),
                Parameters = DynamicDictionary.Empty
            };
        }

        public class TestModel
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

            public DateTime DateProperty { get; set; }
        }
    }
}
