namespace Nancy.Tests.Unit
{
    using System;
    using Xunit;

    public class DefaultModuleActivatorFixture
    {
        DefaultModuleActivator activator;

        public DefaultModuleActivatorFixture()
        {
            this.activator = new DefaultModuleActivator();
        }

        [Fact]
        public void CanCreateInstance_should_return_true_for_module()
        {
            activator.CanCreateInstance(typeof(TestModule)).ShouldBeTrue();
        }

        [Fact]
        public void CanCreateInstance_should_return_false_for_incorrect_baseclass()
        {
            activator.CanCreateInstance(typeof(TestModuleWithoutBaseclass)).ShouldBeFalse();
        }

        [Fact]
        public void CanCreateInstance_should_return_false_when_no_default_constructor()
        {
            activator.CanCreateInstance(typeof(TestModuleWrongCtor)).ShouldBeFalse();
        }

        [Fact]
        public void CreateInstance_creates_instance()
        {
            var instance = activator.CreateInstance(typeof(TestModule)) as TestModule;
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void CreateInstance_throws_for_incorrect_base_class()
        {
            typeof(InvalidOperationException).ShouldBeThrownBy(() => activator.CreateInstance(typeof(TestModuleWithoutBaseclass)));
        }

        private class TestModule : NancyModule
        {
            
        }

        private class TestModuleWithoutBaseclass
        {
            
        }

        private class TestModuleWrongCtor : NancyModule
        {
            public TestModuleWrongCtor(int x)
            {
            }
        }
    }
}