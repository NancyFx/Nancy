namespace Nancy.Tests.Unit.ModelBinding.DefaultConverters
{
    using Nancy.ModelBinding;
    using Nancy.ModelBinding.DefaultConverters;

    using Xunit;

    public class CollectionConverterFixture
    {
        private ITypeConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CollectionConverterFixture()
        {
            this.converter = new CollectionConverter();
        }

        [Fact]
        public void Should_handle_array_types()
        {
            var input = "one,two,three";

            var output = (string[])converter.Convert(input, typeof(string[]), null);

            output.ShouldNotBeNull();
            output.Length.ShouldEqual(3);
        }
    }
}