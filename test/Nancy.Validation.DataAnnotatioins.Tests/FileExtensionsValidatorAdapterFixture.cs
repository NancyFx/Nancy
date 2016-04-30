namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Nancy.Tests;
    using Xunit;

    public class FileExtensionsValidatorAdapterFixture : BaseValidatorAdapterTestFixture<FileExtensionsValidatorAdapter>
    {
        [Fact]
        public void Should_return_true_with_valid_extensions()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel
            {
                Avatar1 = "me.png",
                Avatar2 = "you.jpg",
                Avatar3 = "them.gif"
            };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_read_extension_annotations()
        {
            // Given, When
            var validator = this.factory.Create(typeof(TestModel));

            // Then
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "FileExtension" && r.MemberNames.Contains("Avatar1"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "FileExtension" && r.MemberNames.Contains("Avatar2"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "FileExtension" && r.MemberNames.Contains("Avatar3"));
        }

        [Fact]
        public void Should_return_false_and_contain_property_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { Avatar1 = "me" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["Avatar1"][0].ErrorMessage.ShouldEqual("The Avatar1 field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif");
        }

        [Fact]
        public void Should_return_false_and_contain_display_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { Avatar2 = "you" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["Avatar2"][0].ErrorMessage.ShouldEqual("The Avatar 2 Url field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif");
        }

        [Fact]
        public void Should_return_false_and_contain_displayname()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { Avatar3 = "them" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["Avatar3"][0].ErrorMessage.ShouldEqual("The Avatar 3 Url field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif");
        }

        private class TestModel
        {
            [FileExtensions]
            public string Avatar1 { get; set; }

            [Display(Name = "Avatar 2 Url")]
            [FileExtensions]
            public string Avatar2 { get; set; }

            [DisplayName("Avatar 3 Url")]
            [FileExtensions]
            public string Avatar3 { get; set; }
        }
    }
}
