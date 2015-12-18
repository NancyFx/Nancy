namespace Nancy.Testing.Tests.TestingViewExtensions
{
    using System.Threading.Tasks;
    using Xunit;

    public class GetModuleNameExtensionTests
    {
        private readonly Browser browser;

        public GetModuleNameExtensionTests()
        {
            this.browser = new Browser(with =>
            {
                with.Module<TestingViewFactoryTestModule>();
                with.ViewFactory<TestingViewFactory>();
            });
        }

        [Fact]
        public async Task should_return_name_of_the_module()
        {
            var response = await this.browser.Get("/testingViewFactory");
            Assert.Equal("TestingViewFactoryTest", response.GetModuleName());
        }
    }
}
