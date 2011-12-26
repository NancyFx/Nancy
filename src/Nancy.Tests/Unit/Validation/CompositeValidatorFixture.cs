namespace Nancy.Tests.Unit.Validation
{
    using System.Linq;
    using FakeItEasy;
    using Nancy.Validation;
    using Xunit;

    public class CompositeValidatorFixture
    {
        [Fact]
        public void Should_yield_composite_description()
        {
            // Given
            var fakeValidators = A.CollectionOfFake<IModelValidator>(2);
            A.CallTo(() => fakeValidators[0].Description).Returns(new ModelValidationDescriptor(new[] { new ModelValidationRule("Test1", s => s, new[] { "Member1" }) }));
            A.CallTo(() => fakeValidators[1].Description).Returns(new ModelValidationDescriptor(new[] { new ModelValidationRule("Test2", s => s, new[] { "Member2" }) }));
            var subject = new CompositeValidator(fakeValidators);

            // When
            var result = subject.Description;

            // Then
            result.Rules.ShouldHaveCount(2);
            result.Rules.ShouldHave(r => r.RuleType == "Test1" && r.MemberNames.Contains("Member1"));
            result.Rules.ShouldHave(r => r.RuleType == "Test2" && r.MemberNames.Contains("Member2"));
        }

        [Fact]
        public void Should_invoke_each_validator()
        {
            // Given
            var fakeValidators = A.CollectionOfFake<IModelValidator>(2);
            var subject = new CompositeValidator(fakeValidators);

            // When
            subject.Validate("blah");

            // Then
            A.CallTo(() => fakeValidators[0].Validate(A<object>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeValidators[1].Validate(A<object>.Ignored)).MustHaveHappened();
        }
    }
}