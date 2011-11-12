namespace Nancy.Tests.Unit.Validation.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Validation;
    using Nancy.Validation.DataAnnotations;
    using Xunit;

    public class DataAnnotationValidatorFactoryFixture
    {
        private readonly DataAnnotationsValidatorFactory subject;

        public DataAnnotationValidatorFactoryFixture()
        {
            subject = new DataAnnotationsValidatorFactory();
        }

        [Fact]
        public void Should_provide_null_validator_when_no_rules_exist()
        {
            var result = subject.Create(typeof(string));

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_provide_non_null_validator_when_validation_exists()
        {
            var result = subject.Create(typeof(TestModel));

            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_return_the_same_validator_for_every_call()
        {
            var result = subject.Create(typeof(TestModel));
            var result2 = subject.Create(typeof(TestModel));

            result2.ShouldBeSameAs(result);
        }

        private class TestModel
        {
            [Required]
            public string FirstName { get; set; }
        }
    }
}