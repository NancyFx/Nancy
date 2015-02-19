namespace Nancy.Tests.Unit
{
    using Nancy.Extensions;

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
            NancyModule module = new FakeModule();

            // When
            var name = module.GetModuleName();

            // Then
            name.ShouldEqual("Fake");
        }

        [Fact]
        public void Should_strip_module_from_really_long_name()
        {
            // Given
            NancyModule module = new SuperDuperHappyModule();

            // When
            var name = module.GetModuleName();

            // Then
            name.ShouldEqual("SuperDuperHappy");
        }

        [Fact]
        public void Should_use_fullname_if_module_doesnt_exist()
        {
            // Given
            NancyModule module = new ThisIsNoJoke();

            // When
            var name = module.GetModuleName();

            // Then
            name.ShouldEqual("ThisIsNoJoke");
        }

        [Fact]
        public void Should_use_fullname_if_module_is_not_at_the_end()
        {
            // Given
            NancyModule module = new ModuleForNancy();

            // When
            var name = module.GetModuleName();

            // Then
            name.ShouldEqual("ModuleForNancy");
        }

    }
}
