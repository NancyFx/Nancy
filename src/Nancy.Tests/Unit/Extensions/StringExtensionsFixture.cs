namespace Nancy.Tests.Unit.Extensions
{
    using System;
    using System.Linq;
    using Nancy.Extensions;
    using Xunit;

    public class StringExtensionsFixture
    {
        [Fact]
        public void IsParameterized_should_return_false_when_there_are_parameters()
        {
            "route".IsParameterized().ShouldBeFalse();
        }

        [Fact]
        public void IsParameterized_should_return_true_when_there_is_a_parameter()
        {
            "{param}".IsParameterized().ShouldBeTrue();
        }

        [Fact]
        public void IsParameterized_should_return_false_for_empty_string()
        {
            string.Empty.IsParameterized().ShouldBeFalse();
        }

        //[Fact]
        //public void GetParameterNames_should_throw_format_exception_when_there_are_no_parameters()
        //{
        //    var exception = Record.Exception(() => "route".GetParameterName());
        //    exception.ShouldBeOfType<FormatException>();
        //}

        //[Fact]
        //public void GetParameterNames_should_return_parameter_name()
        //{
        //    "{param}".GetParameterName().First().ShouldEqual("param");
        //}
    }
}