namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Nancy.Configuration;
    using Xunit;

    public class FormatterExtensionsFixture
    {
        private readonly IResponseFormatter formatter;
        private readonly NancyContext context;

        public FormatterExtensionsFixture()
        {
            this.context = new NancyContext();
            this.formatter = new DefaultResponseFormatter(A.Fake<IRootPathProvider>(), context, new DefaultSerializerFactory(null), A.Fake<INancyEnvironment>());
        }

        [Fact]
        public void Should_expand_base_path_for_redirect_if_tilde_present()
        {
            // Given
            this.context.Request = new Request(
                "GET",
                new Url
                    {
                        BasePath = "/basePath",
                        Path = "Path",
                        Scheme = "http",
                    });

            // When
            var result = this.formatter.AsRedirect("~/test");

            // Then
            result.Headers["Location"].ShouldEqual("/basePath/test");
        }

        [Fact]
        public void Should_leave_path_untouched_for_redirect_if_no_tilde()
        {
            // Given
            this.context.Request = new Request(
                "GET",
                new Url
                {
                    BasePath = "/basePath",
                    Path = "Path",
                    Scheme = "http",
                });

            // When
            var result = this.formatter.AsRedirect("/test");

            // Then
            result.Headers["Location"].ShouldEqual("/test");
        }
    }
}