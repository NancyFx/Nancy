namespace Nancy.Testing.Tests.TestingViewExtensions
{
    using Xunit;

    public class GetModulePathExtensionMethodTests
    {
        [Fact]
        public void should_get_the_module_path_for_modules_with_module_path()
        {
            // Arrange
            var browser = new Browser(with =>
            {
                with.Module<TestModuleWithLongModulePath>();
                with.ViewFactory<TestingViewFactory>();
            });

            // Act
            var response = browser.Get("/a/long/path/getModulePath");

            // Assert
            Assert.Equal("/a/long/path", response.GetModulePath());
        }

        [Fact]
        public void GetModule_should_get_empty_string_for_module_with_no_module_path_set()
        {
            // Arrange
            var browser = new Browser(with =>
            {
                with.Module<TestModuleWithLongModulePath>();
                with.ViewFactory<TestingViewFactory>();
            });

            // Act
            var response = browser.Get("/getModulePath");

            // Assert
            Assert.Equal("", response.GetModulePath());
        }

        internal class TestModuleWithLongModulePath : NancyModule
        {
            public TestModuleWithLongModulePath()
                : base("/a/long/path")
            {
                this.Get["/getModulePath"] = _ => this.View["TestingViewExtensions/ViewFactoryTest.sshtml"];
            }
        }

        internal class ModuleWithoutModulePath : NancyModule
        {
            public ModuleWithoutModulePath()
            {
                this.Get["/getModulePath"] = _ => this.View["TestingViewExtensions/ViewFactoryTest.sshtml"];
            }
        }
    }
}