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
                config.StatusCodeHandlers(new Type[] {});
            });
        }


        [Fact] // Works as expected
        public void When_get_sync_then_should_throw()
        {
            Assert.Throws<Exception>(() => this.browser.Get("/sync"));
        }

        [Fact] // Hangs indefinitely
        public void When_get_async_then_should_throw()
        {
            Assert.Throws<Exception>(() => this.browser.Get("/async"));
        }
    }

    public class MyModule : NancyModule
    {
        public MyModule()
        {
            this.Get["/sync"] = _ => { throw new Exception("Derp"); };

            this.Get["/async", true] = (_, __) =>
                Task.Factory.StartNew(() => { })
                    .ContinueWith<dynamic>(t => { throw new Exception("Derp"); });
        }
    }
}