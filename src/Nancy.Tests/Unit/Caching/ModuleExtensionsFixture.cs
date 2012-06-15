
namespace Nancy.Tests.Unit.Caching
{
    using Nancy.Caching;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class ModuleExtensionsFixture
    {
        [Fact]
        public void IncludeETag_should_not_override_the_etag_if_it_is_already_set_in_the_response()
        {
            // Given, when
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.IncludeETag();

            var context = new NancyContext
                              {
                                  Request = new FakeRequest("GET", "/"),
                                  Response = ((Response)"text").WithHeader("ETag", "dummy_etag")
                              };

            module.After.Invoke(context);

            // Then
            context.Response.Headers["ETag"].ShouldEqual("dummy_etag");
        }

        [Fact]
        public void IncludeETag_should_set_the_etag_if_it_does_not_exist_in_the_response()
        {
            // Given, when
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.IncludeETag();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/"),
                Response = "text"
            };

            module.After.Invoke(context);

            // Then
            context.Response.Headers["ETag"].ShouldEqual("1CB251EC0D568DE6A929B520C4AED8D1");
        }

        [Fact]
        public void IncludeETag_should_set_the_etag_if_response_is_empty()
        {
            // Given, when
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.IncludeETag();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/"),
                Response = string.Empty
            };

            module.After.Invoke(context);

            // Then
            context.Response.Headers["ETag"].ShouldEqual("D41D8CD98F00B204E9800998ECF8427E");
        }

    }
}
