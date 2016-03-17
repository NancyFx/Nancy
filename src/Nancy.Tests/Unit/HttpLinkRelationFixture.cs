namespace Nancy.Tests.Unit
{
    using System;

    using Xunit;

    public class HttpLinkRelationFixture
    {
        [Fact]
        public void Parse_null_should_throw_argument_null_exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => HttpLinkRelation.Parse(null));
            exception.ParamName.ShouldEqual("relation");
        }


        [Fact]
        public void Parse_relative_value_should_return_iana_prefixed_link_relation()
        {
            var rel = HttpLinkRelation.Parse("alternate");

            rel.ShouldNotBeNull();
            rel.Prefix.ShouldEqual(HttpLinkRelation.IanaLinkRelationPrefix);
            rel.Value.ShouldEqual("alternate");
        }


        [Fact]
        public void Parse_uri_should_return_link_relation()
        {
            var rel = HttpLinkRelation.Parse("http://nancyfx.org/rels/something");

            rel.ShouldNotBeNull();
            rel.Prefix.ShouldEqual(new Uri("http://nancyfx.org/rels/"));
            rel.Value.ShouldEqual("something");
        }


        [Fact]
        public void Parse_uri_with_path_that_ends_with_slash_throws_format_exception()
        {
            const string uri = "http://nancyfx.org/rels/";

            var exception = Assert.Throws<FormatException>(() => HttpLinkRelation.Parse(uri));
            exception.Message.ShouldContain(uri);
        }


        [Fact]
        public void Parse_uri_without_path_throws_format_exception()
        {
            const string uri = "http://nancyfx.org/";

            var exception = Assert.Throws<FormatException>(() => HttpLinkRelation.Parse(uri));
            exception.Message.ShouldContain(uri);
        }
    }
}
