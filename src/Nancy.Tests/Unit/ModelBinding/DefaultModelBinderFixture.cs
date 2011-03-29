namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using Nancy.ModelBinding;

    using Xunit;

    public class DefaultModelBinderFixture
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
    }
}