
namespace Nancy.Tests.Unit.Caching
{
    using System.Collections.Generic;
    using Nancy;
    using Nancy.Caching;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class ModuleExtensionsFixture
    {
        [Fact]
        public void CheckForIfNonMatch_should_return_not_modified_status_code_if_etag_matches_if_non_match_header()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "If-None-Match", new[] { "8cebd6a8fd10bbc" } 
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfNonMatch();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("ETag", "8cebd6a8fd10bbc")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Fact]
        public void CheckForIfNonMatch_should_not_change_the_status_code_if_etag_does_not_match_if_non_match_header()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "If-None-Match", new[] { "does_not_match_etag" } 
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfNonMatch();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("ETag", "8cebd6a8fd10bbc")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }


        [Fact]
        public void CheckForIfNonMatch_should_not_change_the_status_code_if_etag_is_absent()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "If-None-Match", new[] { "8cebd6a8fd10bbc" } 
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfNonMatch();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = new Response()
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void CheckForIfNonMatch_should_not_change_the_status_code_if_if_non_match_header_is_absent()
        {
            // Given 
            var headers = new Dictionary<string, IEnumerable<string>>();

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfNonMatch();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("ETag", "8cebd6a8fd10bbc")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void CheckForIfModifiedSince_should_return_not_modified_status_code_if_last_modified_matches_if_modified_since()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "If-Modified-Since", new[] { "Sun, 19 Feb 2012 22:11:28 GMT" }
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfModifiedSince();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("Last-Modified", "Sun, 19 Feb 2012 22:11:28 GMT")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Fact]
        public void CheckForIfModifiedSince_should_not_change_the_status_code_if_last_modified_does_not_match_if_modified_since()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                           { "If-Modified-Since" , new[] { "Sun, 9 Feb 2012 22:11:28 GMT" }
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfModifiedSince();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("Last-Modified", "Sun, 19 Feb 2012 22:11:28 GMT")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void CheckForIfModifiedSince_should_not_change_the_status_code_if_last_modified_is_absent()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                           { "If-Modified-Since", new[] { "Sun, 19 Feb 2012 22:11:28 GMT" }
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfModifiedSince();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = new Response()
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void CheckForIfModifiedSince_should_not_change_the_status_code_if_last_modified_since_is_absent()
        {
            // Given 
            var headers = new Dictionary<string, IEnumerable<string>>();

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfModifiedSince();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("Last-Modified", "Sun, 19 Feb 2012 22:11:28 GMT")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void CheckForIfModifiedSince_should_not_change_the_status_code_if_last_modified_is_an_invalid_date()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                           { "If-Modified-Since", new[] { "Sun, 123 Feb 2012 22:11:28 GMT" }
                    }
                };

            // When
            var module = new FakeHookedModule(after: new AfterPipeline());
            module.CheckForIfModifiedSince();

            var context = new NancyContext
            {
                Request = new FakeRequest("GET", "/", headers),
                Response = ((Response)"text").WithHeader("Last-Modified", "Sun, 19 Feb 2012 22:11:28 GMT")
            };

            module.After.Invoke(context);

            // Then
            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }
}
