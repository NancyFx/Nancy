namespace Nancy.Tests.Unit.ModelBinding
{
    using System;

    using Nancy.ModelBinding;

    using Xunit;

    public class PropertyBindingExceptionFixture
    {
        const string PROPERTY_NAME = "PropName";
        const string ATTEMPTED_VALUE = "wrong value";

        [Fact]
        public void Ctor_should_set_property_name_and_attempted_value_and_inner_exception()
        {
            //When
            var innerException = new Exception();
            
            var exception = new PropertyBindingException(PROPERTY_NAME, ATTEMPTED_VALUE, innerException);

            //Then
            exception.PropertyName.ShouldEqual(PROPERTY_NAME);
            exception.AttemptedValue.ShouldEqual(ATTEMPTED_VALUE);
            exception.InnerException.ShouldBeSameAs(innerException);
        }

        [Fact]
        public void Message_should_contain_property_name_and_attempted_value()
        {
            //When
            var exception = new PropertyBindingException(PROPERTY_NAME, ATTEMPTED_VALUE);

            //then
            exception.Message.ShouldEqual(String.Format("Unable to bind property: {0}; Attempted value: {1}", PROPERTY_NAME, ATTEMPTED_VALUE));
        }
    }
}