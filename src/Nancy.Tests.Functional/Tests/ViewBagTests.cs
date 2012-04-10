using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Tests.Functional.Tests
{
    using Bootstrapper;
    using Modules;
    using Testing;
    using Xunit;

    public class ViewBagTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public ViewBagTests()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                    configuration => configuration.Modules(new Type[] { typeof(RazorTestModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_render_content_from_viewbag()
        {
            var response = browser.Get(
                @"/razor-viewbag",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.True(response.Body.AsString().Contains(@"Hello Bob"));
        }
    }
}
