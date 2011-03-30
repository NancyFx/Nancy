namespace Nancy.Testing.Tests
{
    using Nancy.Testing.Fakes;

    public class BrowserFixture
    {
        private readonly Browser browser;

        public BrowserFixture()
        {
            var bootstrapper = 
                new FakeNancyBootstrapper();

            this.browser = new Browser(bootstrapper);
        }
    }
}