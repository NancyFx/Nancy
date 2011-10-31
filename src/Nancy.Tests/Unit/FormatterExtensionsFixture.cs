namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Xunit;

    public class FormatterExtensionsFixture
    {
        private readonly IResponseFormatter formatter;
        private readonly NancyContext context;

        public FormatterExtensionsFixture()
        {
            this.context = new NancyContext();
            this.formatter = new DefaultResponseFormatter(A.Fake<IRootPathProvider>(), context, new ISerializer[] { });
        }

        [Fact]
        public void Should_expand_base_path_for_redirect_if_tilde_present()
        {
            this.context.Request = new Request(
                "GET", 
                new Url
                    {
                        BasePath = "/basePath",
                        Path = "Path",
                        Scheme = "http",
                    });

            var result = this.formatter.AsRedirect("~/test");

            result.Headers["Location"].ShouldEqual("/basePath/test");
        }

        [Fact]
        public void Should_leave_path_untouched_for_redirect_if_no_tilde()
        {
            this.context.Request = new Request(
                "GET",
                new Url
                {
                    BasePath = "/basePath",
                    Path = "Path",
                    Scheme = "http",
                });

            var result = this.formatter.AsRedirect("/test");

            result.Headers["Location"].ShouldEqual("/test");
        }
    }
}