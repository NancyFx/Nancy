namespace Nancy.Tests.Unit.Validation
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Validation;
    using Xunit;

    public class CompositeValidatorFixture
    {
        [Fact]
        public void Should_yield_composite_description()
        {
            var fakeValidators = A.CollectionOfFake<IValidator>(2);
            A.CallTo(() => fakeValidators[0].Description).Returns(new ValidationDescriptor(new [] { new ValidationRule("Test1", s => s, new [] { "Member1" }) }));
            A.CallTo(() => fakeValidators[1].Description).Returns(new ValidationDescriptor(new [] { new ValidationRule("Test2", s => s, new [] { "Member2" }) }));
            var subject = new CompositeValidator(fakeValidators);

            var result = subject.Description;

            result.Rules.ShouldHaveCount(2);
            result.Rules.ShouldHave(r => r.RuleType == "Test1" && r.MemberNames.Contains("Member1"));
            result.Rules.ShouldHave(r => r.RuleType == "Test2" && r.MemberNames.Contains("Member2"));
        }

        [Fact]
        public void Should_invoke_each_validator()
        {
            var fakeValidators = A.CollectionOfFake<IValidator>(2);
            var subject = new CompositeValidator(fakeValidators);

            subject.Validate("blah");

            A.CallTo(() => fakeValidators[0].Validate(A<object>.Ignored)).MustHaveHappened();
            A.CallTo(() => fakeValidators[1].Validate(A<object>.Ignored)).MustHaveHappened();
        }
    }
}