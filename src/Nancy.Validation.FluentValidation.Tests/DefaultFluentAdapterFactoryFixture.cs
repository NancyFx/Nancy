namespace Nancy.Validation.FluentValidation.Tests
{
    using FakeItEasy;
    using Nancy.Tests;
    using Xunit;
    using global::FluentValidation.Internal;
    using global::FluentValidation.Validators;

    public class DefaultFluentAdapterFactoryFixture
    {
        private PropertyRule rule;
        private DefaultFluentAdapterFactory factory;

        public DefaultFluentAdapterFactoryFixture()
        {
            this.rule = new PropertyRule(null, null, null, null, null, null);
            this.factory = new DefaultFluentAdapterFactory();
        }

        [Fact]
        public void Should_create_custom_adapter_for_unknown_validator()
        {
            // Given
            var validator = A.Fake<IPropertyValidator>();

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<FluentAdapter>();
        }

        [Fact]
        public void Should_create_emailadapter_for_emailvalidator()
        {
            // Given
            var validator = new EmailValidator();

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<EmailAdapter>();
        }

        [Fact]
        public void Should_create_equaladapter_for_equalvalidator()
        {
            // Given
            var validator = new EqualValidator(10);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<EqualAdapter>();
        }

        [Fact]
        public void Should_create_exactlengthadapter_for_exactlengthvalidator()
        {
            // Given
            var validator = new ExactLengthValidator(10);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<ExactLengthAdapater>();
        }

        [Fact]
        public void Should_create_exclusivebetweenadapter_for_exclusivebetweenvalidator()
        {
            // Given
            var validator = new ExclusiveBetweenValidator(1, 10);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<ExclusiveBetweenAdapter>();
        }

        [Fact]
        public void Should_create_greaterthanadapter_for_greaterthanvalidator()
        {
            // Given
            var validator = new GreaterThanValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<GreaterThanAdapter>();
        }

        [Fact]
        public void Should_create_greaterthanorequaladapter_for_greaterthanorequalvalidator()
        {
            // Given
            var validator = new GreaterThanOrEqualValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<GreaterThanOrEqualAdapter>();
        }

        [Fact]
        public void Should_create_inclusivebetweenadapter_for_inclusivebetweenvalidator()
        {
            // Given
            var validator = new InclusiveBetweenValidator(1, 10);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<InclusiveBetweenAdapter>();
        }

        [Fact]
        public void Should_create_lengthadapter_for_lengthvalidator()
        {
            // Given
            var validator = new LengthValidator(1, 10);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<LengthAdapter>();
        }

        [Fact]
        public void Should_create_lessthanadapter_for_lessthanvalidator()
        {
            // Given
            var validator = new LessThanValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<LessThanAdapter>();
        }

        [Fact]
        public void Should_create_lessthanorequaladapter_for_lessthanorequalvalidator()
        {
            // Given
            var validator = new LessThanOrEqualValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<LessThanOrEqualAdapter>();
        }

        [Fact]
        public void Should_create_notemptyadapter_for_notemptyvalidator()
        {
            // Given
            var validator = new NotEmptyValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<NotEmptyAdapter>();
        }

        [Fact]
        public void Should_create_notequaladapter_for_notequalvalidator()
        {
            // Given
            var validator = new NotEqualValidator(1);

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<NotEqualAdapter>();
        }

        [Fact]
        public void Should_create_notnulladapter_for_notnullvalidator()
        {
            // Given
            var validator = new NotNullValidator();

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<NotNullAdapter>();
        }

        [Fact]
        public void Should_create_regularexpressionadapter_for_regularexpressionvalidator()
        {
            // Given
            var validator = new RegularExpressionValidator("[A-Z]*");

            // When
            var result = factory.Create(this.rule, validator);

            // Then
            result.ShouldBeOfType<RegularExpressionAdapter>();
        }
    }
}