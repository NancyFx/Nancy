namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Threading.Tasks;

    using Nancy.Testing;
    using Nancy.Tests.xUnitExtensions;
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
            var ex = await RecordAsync.Exception(async () => await this.browser.Get("/sync"));

            Assert.NotNull(ex);
        }

        [Fact]
        public async Task When_get_async_then_should_throw()
        {
            var ex = await RecordAsync.Exception(async () => await this.browser.Get("/sync"));

            Assert.NotNull(ex);
        }
    }

    public class MyModule : LegacyNancyModule
    {
        public MyModule()
        {
            this.Get["/sync"] = _ => { throw new Exception("Derp"); };

            this.Get["/async", true] = (_, __) =>
                Task.Factory.StartNew(() => { }) // Force continuation on a worker thread
                    .ContinueWith<dynamic>(t => { throw new Exception("Derp"); });
        }
    }
}