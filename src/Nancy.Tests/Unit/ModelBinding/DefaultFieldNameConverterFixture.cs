namespace Nancy.Tests.Unit.ModelBinding
{
    using Nancy.ModelBinding;

    using Xunit;

    public class DefaultFieldNameConverterFixture
    {
        private readonly DefaultFieldNameConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DefaultFieldNameConverterFixture()
        {
            this.converter = new DefaultFieldNameConverter();
        }

        [Fact]
        public void Should_just_return_name_if_pascal_cased()
        {
            var input = "FieldName";

            var result = this.converter.Convert(input);

            result.ShouldEqual(input);
        }

        [Fact]
        public void Should_pascal_case_camel_case_field()
        {
            var input = "fieldName";

            var result = this.converter.Convert(input);

            result.ShouldEqual("FieldName");
        }
    }
}