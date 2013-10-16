//namespace Nancy.Validation.DataAnnotatioins.Tests
//{
//    using System.ComponentModel.DataAnnotations;
//    using Nancy.Tests;
//    using Nancy.Validation.DataAnnotations;
//    using Xunit;

//    public class DataAnnotationValidatorFactoryFixture
//    {
//        private readonly DataAnnotationsValidatorFactory subject;

//        public DataAnnotationValidatorFactoryFixture()
//        {
//            this.subject = new DataAnnotationsValidatorFactory();
//        }

//        [Fact]
//        public void Should_provide_null_validator_when_no_rules_exist()
//        {
//            // Given, When
//            var result = this.subject.Create(typeof(string));

//            // Then
//            result.ShouldBeNull();
//        }

//        [Fact]
//        public void Should_provide_non_null_validator_when_validation_exists()
//        {
//            // Given, When
//            var result = this.subject.Create(typeof(TestModel));

//            // Then
//            result.ShouldNotBeNull();
//        }

//        private class TestModel
//        {
//            [Required]
//            public string FirstName { get; set; }
//        }
//    }
//}