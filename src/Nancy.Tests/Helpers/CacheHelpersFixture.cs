namespace Nancy.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Nancy.Helpers;

    using Xunit;

    public class CacheHelpersFixture
    {
        private string etag;

        private DateTime lastModified;

        public CacheHelpersFixture()
        {
            this.etag = "etag";
            this.lastModified = DateTime.Now;
        }
        
        [Fact]
        public void Should_return_modified_false_if_no_etag_and_no_ifmodifiedsince_sent()
        {
            var context = this.GetContext(null, null);

            var result = CacheHelpers.ReturnNotModified(this.etag, this.lastModified, context);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_with_differe_etag()
        {
            var context = this.GetContext("moo", null);

            var result = CacheHelpers.ReturnNotModified(this.etag, this.lastModified, context);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_with_same_etag()
        {
            var context = this.GetContext(this.etag, null);

            var result = CacheHelpers.ReturnNotModified(this.etag, this.lastModified, context);

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_with_date_in_the_past()
        {
            var context = this.GetContext(null, this.lastModified.AddMinutes(-1));

            var result = CacheHelpers.ReturnNotModified(this.etag, this.lastModified, context);

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_with_date_in_future()
        {
            var context = this.GetContext(null, this.lastModified.AddHours(1));

            var result = CacheHelpers.ReturnNotModified(this.etag, this.lastModified, context);

            result.ShouldBeTrue();
        }

        private NancyContext GetContext(string etag, DateTime? lastModified)
        {
            var headers = new Dictionary<string, IEnumerable<string>>();
            if (!string.IsNullOrEmpty(etag))
            {
                headers["If-None-Match"] = new[] { etag };
            }
            if (lastModified.HasValue)
            {
                headers["If-Modified-Since"] = new[] { lastModified.Value.ToString("R", CultureInfo.InvariantCulture) };
            }

            var request = new Request("GET", new Url { Path = "/", Scheme = "http" }, headers: headers);

            var context = new NancyContext { Request = request };

            return context;
        }
    }
}
