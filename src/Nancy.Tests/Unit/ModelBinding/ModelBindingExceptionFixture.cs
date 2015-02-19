namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.ModelBinding;

    using Xunit;

    public class ModelBindingExceptionFixture
    {
        [Fact]
        public void Ctor_should_set_property_exceptions_and_bound_type()
        {
            //When
            var propertyExceptions = new List<PropertyBindingException>();
            var exception = new ModelBindingException(typeof (string), propertyExceptions);

            //Then
            exception.BoundType.ShouldBeOfType<string>();
            exception.PropertyBindingExceptions.ShouldBeSameAs(propertyExceptions);
        }

        [Fact]
        public void Ctor_should_set_empty_property_exceptions_list_if_null_is_provided()
        {
            //When
            var exception = new ModelBindingException(typeof(string), null);

            //Then
            exception.PropertyBindingExceptions.Any().ShouldBeFalse();
        }

        [Fact]
        public void Ctor_should_set_empty_property_exceptions_list_if_none_are_provided()
        {
            //When
            var exception = new ModelBindingException(typeof (string));

            //Then
            exception.PropertyBindingExceptions.Any().ShouldBeFalse();
        }

        [Fact]
        public void Ctor_should_throw_on_null_type()
        {
            Assert.Throws<ArgumentNullException>(() => new ModelBindingException(null))
                .ParamName.ShouldEqual("boundType");
        }

        [Fact]
        public void Message_should_contain_bound()
        {
            //When
            var exception = new ModelBindingException(typeof (string));

            //then
            exception.Message.ShouldEqual(String.Format("Unable to bind to type: {0}", typeof(string)));
        }
    }
}