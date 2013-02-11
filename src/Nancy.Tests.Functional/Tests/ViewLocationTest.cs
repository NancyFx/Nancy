namespace Nancy.Tests.Functional.Tests
{
    using System;
    using Nancy.Bootstrapper;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;
    using Xunit;

    public class ViewLocationTest
    {
        readonly INancyBootstrapper bootstrapper;

        readonly Browser browser;

        public ViewLocationTest()
        {
            this.bootstrapper = new ConfigurableBootstrapper(
                configuration => configuration.Modules(new Type[] {typeof (ViewLocationTestModule)}));

            this.browser = new Browser(bootstrapper);
        }


        [Fact]
        public void Should_render_content_using_convention_0()
        {
            // Given
            // When
            var response = browser.Get(
                @"/viewconvention0/test",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.True(response.Body.AsString().Contains(@"0 Handles: views / *modulepath* / *modulename* / *viewname*"));
        }

        [Fact]
        public void Should_render_content_using_convention_2()
        {
            // Given
            // When
            var response = browser.Get(
                @"/viewconvention2/test",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.True(response.Body.AsString().Contains(@"2 Handles: views / *modulepath* / *viewname*"));
        }

        [Fact]
        public void Should_render_content_using_convention_4()
        {
            // Given
            // When
            var response = browser.Get(
                @"/viewconvention4/test",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.True(response.Body.AsString().Contains(@"4 Handles: views / *modulename* / *viewname*"));
        }

        [Fact]
        public void Should_render_content_using_convention_6()
        {
            // Given
            // When
            var response = browser.Get(
                @"/viewconvention6/testconvention6",
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.True(response.Body.AsString().Contains(@"6 Handles: views / *viewname*"));
        }
    }
}