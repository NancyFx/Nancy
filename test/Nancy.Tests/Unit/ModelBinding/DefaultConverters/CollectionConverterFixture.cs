namespace Nancy.Tests.Unit.ModelBinding.DefaultConverters
{
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using Nancy.ModelBinding;
    using Nancy.ModelBinding.DefaultConverters;

    using Xunit;

    public class CollectionConverterFixture
    {
        private ITypeConverter converter;
        private BindingContext context;
        private ITypeConverter mockStringTypeConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CollectionConverterFixture()
        {
            this.converter = new CollectionConverter();
            this.context = new BindingContext() { TypeConverters = new[] { new FallbackConverter() } };

            this.mockStringTypeConverter = A.Fake<ITypeConverter>();
            A.CallTo(() => mockStringTypeConverter.CanConvertTo(null, null)).WithAnyArguments().Returns(true);
            A.CallTo(() => mockStringTypeConverter.Convert(null, null, null)).WithAnyArguments().Returns(string.Empty);
        }

        [Fact]
        public void Should_handle_array_types()
        {
            const string input = "one,two,three";

            var output = (string[])converter.Convert(input, typeof(string[]), this.context);

            output.ShouldNotBeNull();
            output.Length.ShouldEqual(3);
        }

        [Fact]
        public void Array_type_conversion_should_use_type_converter()
        {
            const string input = "one,two,three";
            var mockContext = new BindingContext() { TypeConverters = new[] { this.mockStringTypeConverter } };

            converter.Convert(input, typeof(string[]), mockContext);

            A.CallTo(() => this.mockStringTypeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void Should_handle_collection_types()
        {
            const string input = "one,two,three";

            var output = (List<string>)converter.Convert(input, typeof(List<string>), this.context);

            output.ShouldNotBeNull();
            output.Count.ShouldEqual(3);
        }

        [Fact]
        public void Collection_type_conversion_should_use_type_converter()
        {
            const string input = "one,two,three";
            var mockContext = new BindingContext() { TypeConverters = new[] { this.mockStringTypeConverter } };

            converter.Convert(input, typeof(List<string>), mockContext);

            A.CallTo(() => this.mockStringTypeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void Should_handle_IEnumerable_types()
        {
            const string input = "one,two,three";

            var output = (IEnumerable<string>)converter.Convert(input, typeof(IEnumerable<string>), this.context);

            output.ShouldNotBeNull();
            output.Count().ShouldEqual(3);
        }

        [Fact]
        public void IEnumerable_type_conversion_should_use_type_converter()
        {
            const string input = "one,two,three";
            var mockContext = new BindingContext() { TypeConverters = new[] { this.mockStringTypeConverter } };

            converter.Convert(input, typeof(IEnumerable<string>), mockContext);

            A.CallTo(() => this.mockStringTypeConverter.Convert(null, null, null)).WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(3));
        }
    }
}