namespace Nancy.Testing.Tests.TestingViewExtensions
{
    using System.Threading.Tasks;
    using Xunit;

    public class GetModulePathExtensionMethodTests
    {
        [Fact]
        public async Task should_get_the_module_path_for_modules_with_module_path()
        {
            // Arrange
            var browser = new Browser(with =>
            {
                with.Module<TestModuleWithLongModulePath>();
                with.ViewFactory<TestingViewFactory>();
            });

            // Act
            var response = await browser.Get("/a/long/path/getModulePath");

            // Assert
            Assert.Equal("/a/long/path", response.GetModulePath());
        }

        [Fact]
        public async Task GetModule_should_get_empty_string_for_module_with_no_module_path_set()
        {
            // Arrange
            var browser = new Browser(with =>
            {
                with.Module<TestModuleWithLongModulePath>();
                with.ViewFactory<TestingViewFactory>();
            });

            // Act
            var response = await browser.Get("/getModulePath");

            // Assert
            Assert.Equal("", response.GetModulePath());
        }

        internal class TestModuleWithLongModulePath : NancyModule
        {
            public TestModuleWithLongModulePath()
                : base("/a/long/path")
            {
                Get("/getModulePath", args => this.View["TestingViewExtensions/ViewFactoryTest.sshtml"]);
            }
        }

        internal class ModuleWithoutModulePath : NancyModule
        {
            public ModuleWithoutModulePath()
            {
                Get("/getModulePath", args => this.View["TestingViewExtensions/ViewFactoryTest.sshtml"]);
            }
        }
    }
}