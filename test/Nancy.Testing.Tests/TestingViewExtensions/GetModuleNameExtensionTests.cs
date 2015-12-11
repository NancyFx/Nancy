namespace Nancy.Testing.Tests.TestingViewExtensions
{
    using Xunit;

    public class GetModuleNameExtensionTests
    {
        private readonly Browser _browser;

        public GetModuleNameExtensionTests()
        {
            this._browser = new Browser(with =>
            {
                with.Module<TestingViewFactoryTestModule>();
                with.ViewFactory<TestingViewFactory>();
            });
        }

        [Fact]
        public void should_return_name_of_the_module()
        {
            var response = this._browser.Get("/testingViewFactory");
            Assert.Equal("TestingViewFactoryTest", response.GetModuleName());
        }
    }
}
