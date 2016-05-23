namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;

    using Nancy.Testing;
    using Xunit;

    public class AsyncExceptionTests
    {
        private readonly Browser browser;

        public AsyncExceptionTests()
        {
            this.browser = new Browser(config =>
            {
                config.Module<MyModule>();
                // explicitly declare the status code handler that throws, just
                // incase it's not there for some other reason.
                config.StatusCodeHandlers(new[] { typeof(PassThroughStatusCodeHandler)});
            });
        }

        [Fact]
        public async Task When_get_sync_then_should_throw()
        {
            var ex = await Assert.ThrowsAsync<Exception>(() => this.browser.Get("/sync"));

            // Unwrap exception from PassThroughStatusCodeHandler
            ex = ex.InnerException.InnerException;

            Assert.NotNull(ex);
            Assert.IsType<Exception>(ex);
            Assert.Equal("Derp", ex.Message);
        }

        [Fact]
        public async Task When_get_async_then_should_throw()
        {
            var ex = await Assert.ThrowsAsync<Exception>(() => this.browser.Get("/async"));

            // Unwrap exception from PassThroughStatusCodeHandler
            ex = ex.InnerException.InnerException;

            Assert.NotNull(ex);
            Assert.IsType<Exception>(ex);
            Assert.Equal("Derp", ex.Message);
        }
    }

    public class MyModule : NancyModule
    {
        public MyModule()
        {
            Get("/sync", args =>
            {
                throw new Exception("Derp");
                return 500;
            });

            Get("/async", async args =>
            {
                throw new Exception("Derp");
                return await Task.FromResult(200);
            });
        }
    }
}
