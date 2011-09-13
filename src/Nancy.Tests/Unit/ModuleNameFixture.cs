namespace Nancy.Tests.Unit
{
    using Xunit;

    public class ModuleNameFixture
    {
        class FakeModule : NancyModule
        {
            
        }

        class SuperDuperHappyModule : NancyModule
        {
            
        }

        class ThisIsNoJoke : NancyModule
        {
            
        }

        class ModuleForNancy : NancyModule
        {
            
        }

        [Fact]
        public void Should_strip_module_from_name()
        {
            // Given
            NancyModule module;

            // When
            module = new FakeModule();

            // Then
            module.Name.ShouldEqual("Fake");
        }

        [Fact]
        public void Should_strip_module_from_really_long_name()
        {
            // Given
            NancyModule module;

            // When
            module = new SuperDuperHappyModule();

            // Then
            module.Name.ShouldEqual("SuperDuperHappy");
        }

        [Fact]
        public void Should_use_fullname_if_module_doesnt_exist()
        {
            // Given
            NancyModule module;

            // When
            module = new ThisIsNoJoke();

            // Then
            module.Name.ShouldEqual("ThisIsNoJoke");
        }

        [Fact]
        public void Should_use_fullname_if_module_is_not_at_the_end()
        {
            // Given
            NancyModule module;

            // When
            module = new ModuleForNancy();

            // Then
            module.Name.ShouldEqual("ModuleForNancy");
        }

    }
}
