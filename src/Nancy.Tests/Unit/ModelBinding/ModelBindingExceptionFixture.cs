namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using Nancy.ModelBinding;
    using Xunit;

    public class ModelBindingExceptionFixture
    {
        private const string PROPNAME = "Property";

        [Fact]
        public void Ctor_should_set_property_name_and_bound_type_and_inner_exception()
        {
            //When
            var inner = new Exception();
            var exception = new ModelBindingException(typeof (string), "Length", inner);

            //Then
            exception.BoundType.ShouldBeOfType<string>();
            exception.PropertyName.ShouldEqual("Length");
            exception.InnerException.ShouldBeSameAs(inner);
        }

        [Fact]
        public void Message_should_contain_bound_type_and_property_name()
        {
            //When
            var exception = new ModelBindingException(typeof (string), "PropName");

            //then
            exception.Message.ShouldEqual(String.Format("Unable to bind to type: {0}; Property: {1}", typeof(string), "PropName"));
        }

        [Fact]
        public void Message_should_contain_only_bound_type_if_no_property_is_provided()
        {
            //When
            var exception = new ModelBindingException(typeof (string));

            //Then
            exception.Message.ShouldEqual("Unable to bind to type: " + typeof(string));
        }
    }
}