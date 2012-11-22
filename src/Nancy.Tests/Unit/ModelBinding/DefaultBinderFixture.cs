namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using FakeItEasy;

    using Nancy.IO;
    using Nancy.ModelBinding;
    using Fakes;

    using Nancy.ModelBinding.DefaultBodyDeserializers;
    using Nancy.ModelBinding.DefaultConverters;
    using Nancy.Tests.Unit.ModelBinding.DefaultBodyDeserializers;

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
            binder.Bind(context, this.GetType(), null, new BindingConfig());

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
            binder.Bind(context, this.GetType(), null, new BindingConfig());

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
            binder.Bind(context, this.GetType(), null, new BindingConfig());

            // Then
            A.CallTo(() => deserializer.CanDeserialize("application/xml"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_use_object_from_deserializer_if_one_returned()
        {
            // Given
            var modelObject = new TestModel { StringProperty = "Hello!" };
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments().Returns(modelObject);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            var result = binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.ShouldBeOfType<TestModel>();
            ((TestModel)result).StringProperty.ShouldEqual("Hello!");
        }

        [Fact]
        public void Should_use_object_from_deserializer_if_one_returned_and_overwrite_when_allowed()
        {
            // Given
            var modelObject = new TestModel { StringPropertyWithDefaultValue = "Hello!" };
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments().Returns(modelObject);
            var binder = this.GetBinder(bodyDeserializers: new[] { deserializer });

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            var result = binder.Bind(context, typeof(TestModel), null, BindingConfig.Default);

            // Then
            result.ShouldBeOfType<TestModel>();
            ((TestModel)result).StringPropertyWithDefaultValue.ShouldEqual("Hello!");
        }

        [Fact]
        public void Should_use_object_from_deserializer_if_one_returned_and_not_overwrite_when_not_allowed()
        {
            // Given
            var modelObject = new TestModel {StringPropertyWithDefaultValue = "Hello!"};
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(null)).WithAnyArguments().Returns(true);
            A.CallTo(() => deserializer.Deserialize(null, null, null)).WithAnyArguments().Returns(modelObject);
            var binder = this.GetBinder(bodyDeserializers: new[] {deserializer});

            var context = CreateContextWithHeader("Content-Type", new[] {"application/xml"});

            // When
            var result = binder.Bind(context, typeof (TestModel), null, BindingConfig.NoOverwrite);

            // Then
            result.ShouldBeOfType<TestModel>();
            ((TestModel) result).StringPropertyWithDefaultValue.ShouldEqual("Default Value");
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
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

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
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

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
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
            result.DateProperty.ShouldEqual(default(DateTime));
        }

        [Fact]
        public void Should_throw_ModelBindingException_if_convertion_of_a_property_fails()
        {
            // Given
            var binder = this.GetBinder(typeConverters: new[] { new FallbackConverter() });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["IntProperty"] = "badint";
            context.Request.Form["AnotherIntProperty"] = "morebad";

            // Then
            Assert.Throws<ModelBindingException>(() => binder.Bind(context, typeof(TestModel), null, new BindingConfig()))
                .ShouldMatch(exception =>
                             exception.BoundType == typeof(TestModel)
                             && exception.PropertyBindingExceptions.Any(pe =>
                                                                        pe.PropertyName == "IntProperty"
                                                                        && pe.AttemptedValue == "badint"
                                                                        && pe.InnerException.Message == "badint is not a valid value for Int32.")
                             && exception.PropertyBindingExceptions.Any(pe =>
                                                                        pe.PropertyName == "AnotherIntProperty"
                                                                        && pe.AttemptedValue == "morebad"
                                                                        && pe.InnerException.Message == "morebad is not a valid value for Int32."));
        }

        [Fact]
        public void Should_ignore_indexer_properties()
        {
            // Given
            var binder = this.GetBinder(typeConverters: new[] { new FallbackConverter() });
            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            var validProperties = 0;
            var deserializer = A.Fake<IBodyDeserializer>();
            A.CallTo(() => deserializer.CanDeserialize(A<string>.Ignored)).Returns(true);
            A.CallTo(() => deserializer.Deserialize(A<string>.Ignored, A<Stream>.Ignored, A<BindingContext>.Ignored))
                                       .Invokes(f =>
                                           {
                                               validProperties = f.Arguments.Get<BindingContext>(2).ValidModelProperties.Count();
                                           })
                                       .Returns(new TestModel());

            A.CallTo(() => this.emptyDefaults.DefaultBodyDeserializers).Returns(new[] { deserializer });

            // When
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            validProperties.ShouldEqual(6);
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
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

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
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig(), "IntProperty");

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
            binder.Bind(context, this.GetType(), null, new BindingConfig());

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
            var binder = this.GetBinder(new ITypeConverter[] { });
            var context = new NancyContext { Request = new FakeRequest("GET", "/") };
            context.Request.Form["StringProperty"] = "Test";

            // When
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

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
            binder.Bind(context, this.GetType(), null, new BindingConfig());

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
            binder.Bind(context, typeof(TestModel), null, new BindingConfig());

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
            context.Request.Query["IntProperty"] = "3";


            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(3);
        }

        [Fact]
        public void Should_bind_model_from_context_parameters()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Parameters["StringProperty"] = "Test";
            context.Parameters["IntProperty"] = "3";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(3);
        }

        [Fact]
        public void Form_properties_should_take_precendence_over_request_properties()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "3";
            context.Request.Query["StringProperty"] = "Test2";
            context.Request.Query["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(3);
        }

        [Fact]
        public void Form_properties_should_take_precendence_over_request_properties_and_context_properties()
        {
            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Form["IntProperty"] = "3";
            context.Request.Query["StringProperty"] = "Test2";
            context.Request.Query["IntProperty"] = "1";
            context.Parameters["StringProperty"] = "Test3";
            context.Parameters["IntProperty"] = "2";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(3);
        }

        [Fact]
        public void Request_properties_should_take_precendence_over_context_properties()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Request.Query["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "12";
            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "13";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());
            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
        }

        [Fact]
        public void Should_be_able_to_bind_from_form_and_request_simultaneously()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Form["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "12";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
        }

        [Fact]
        public void Should_be_able_to_bind_from_request_and_context_simultaneously()
        {

            var binder = this.GetBinder();

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });
            context.Request.Query["StringProperty"] = "Test";
            context.Parameters["IntProperty"] = "12";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(12);
        }

        [Fact]
        public void Should_not_overwrite_nullable_property_if_already_set_and_overwriting_is_not_allowed()
        {
            var binder = this.GetBinder();
            var existing = new TestModel { StringProperty = "Existing Value" };

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Request.Query["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "12";
            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), existing, BindingConfig.NoOverwrite);

            // Then
            result.StringProperty.ShouldEqual("Existing Value");
            result.IntProperty.ShouldEqual(12);
        }

        [Fact]
        public void Should_not_overwrite_non_nullable_property_if_already_set_and_overwriting_is_not_allowed()
        {
            var binder = this.GetBinder();
            var existing = new TestModel { IntProperty = 27 };

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Request.Query["StringProperty"] = "Test";
            context.Request.Query["IntProperty"] = "12";
            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), existing, BindingConfig.NoOverwrite);

            // Then
            result.StringProperty.ShouldEqual("Test");
            result.IntProperty.ShouldEqual(27);
        }

        [Fact]
        public void Should_overwrite_nullable_property_if_already_set_and_overwriting_is_allowed()
        {
            var binder = this.GetBinder();
            var existing = new TestModel { StringProperty = "Existing Value" };

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), existing, BindingConfig.Default);

            // Then
            result.StringProperty.ShouldEqual("Test2");
            result.IntProperty.ShouldEqual(1);
        }

        [Fact]
        public void Should_overwrite_non_nullable_property_if_already_set_and_overwriting_is_allowed()
        {
            var binder = this.GetBinder();
            var existing = new TestModel { IntProperty = 27 };

            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            context.Parameters["StringProperty"] = "Test2";
            context.Parameters["IntProperty"] = "1";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), existing, BindingConfig.Default);

            // Then
            result.StringProperty.ShouldEqual("Test2");
            result.IntProperty.ShouldEqual(1);
        }

        [Fact]
        public void Form_request_and_context_properties_should_take_precedence_over_body_properties()
        {

            var typeConverters = new ITypeConverter[] { new CollectionConverter(), new FallbackConverter(), };
            var binder = this.GetBinder(typeConverters);
            var body = XmlBodyDeserializerFixture.ToXmlString(new TestModel() { IntProperty = 0, StringProperty = "From body" });

            var context = CreateContextWithHeaderAndBody("Content-Type", new[] { "application/xml" }, body);

            context.Request.Form["StringProperty"] = "From form";
            context.Request.Query["IntProperty"] = "1";
            context.Parameters["AnotherStringProprety"] = "From context";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.StringProperty.ShouldEqual("From form");
            result.AnotherStringProprety.ShouldEqual("From context");
            result.IntProperty.ShouldEqual(1);
        }

        [Fact]
        public void Should_be_able_to_bind_body_request_form_and_context_properties()
        {
            var binder = this.GetBinder(null, new List<IBodyDeserializer> { new XmlBodyDeserializer() });
            var body = XmlBodyDeserializerFixture.ToXmlString(new TestModel { DateProperty = new DateTime(2012, 8, 16) });

            var context = CreateContextWithHeaderAndBody("Content-Type", new[] { "application/xml" }, body);

            context.Request.Form["IntProperty"] = "0";
            context.Request.Query["StringProperty"] = "From Query";
            context.Parameters["AnotherStringProprety"] = "From Context";

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), null, new BindingConfig());

            // Then
            result.StringProperty.ShouldEqual("From Query");
            result.IntProperty.ShouldEqual(0);
            result.DateProperty.ShouldEqual(new DateTime(2012, 8, 16));
            result.AnotherStringProprety.ShouldEqual("From Context");
        }

        [Fact]
        public void Should_ignore_existing_instance_if_type_doesnt_match()
        {
            var binder = this.GetBinder();
            var existing = new object();
            var context = CreateContextWithHeader("Content-Type", new[] { "application/xml" });

            // When
            var result = (TestModel)binder.Bind(context, typeof(TestModel), existing, new BindingConfig());

            // Then
            result.ShouldNotBeSameAs(existing);
        }

        private IBinder GetBinder(IEnumerable<ITypeConverter> typeConverters = null, IEnumerable<IBodyDeserializer> bodyDeserializers = null, IFieldNameConverter nameConverter = null, BindingDefaults bindingDefaults = null)
        {
            var converters = typeConverters ?? new ITypeConverter[] { new FallbackConverter(), };
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

        private static NancyContext CreateContextWithHeaderAndBody(string name, IEnumerable<string> values, string body)
        {
            var header = new Dictionary<string, IEnumerable<string>>
            {
                { name, values }
            };

            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            var bodyStream = RequestStream.FromStream(new MemoryStream(byteArray));

            return new NancyContext
            {
                Request = new FakeRequest("GET", "/", header, bodyStream, "http", string.Empty),
                Parameters = DynamicDictionary.Empty
            };
        }

        public class TestModel
        {
            public TestModel()
            {
                this.StringPropertyWithDefaultValue = "Default Value";
            }

            public string StringProperty { get; set; }

            public string AnotherStringProprety { get; set; }

            public int IntProperty { get; set; }

            public int AnotherIntProperty { get; set; }

            public DateTime DateProperty { get; set; }

            public string StringPropertyWithDefaultValue { get; set; }

            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }
    }

    public class BindingConfigFixture
    {
        [Fact]
        public void Should_allow_overwrite_on_new_instance()
        {
            // Given
            // When
            var instance = new BindingConfig();

            // Then
            instance.Overwrite.ShouldBeTrue();
        }
    }
}
