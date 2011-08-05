namespace Nancy.Testing.Tests
{
    public class BrowserFixture
    {
        private readonly Browser browser;

        public BrowserFixture()
        {
            var bootstrapper = 
                new ConfigurableBootstrapper();

            this.browser = new Browser(bootstrapper);
        }
    }
}