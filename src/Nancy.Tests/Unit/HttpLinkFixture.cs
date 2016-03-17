namespace Nancy.Tests.Unit
{
    using System;

    using Xunit;

    public class HttpLinkFixture
    {
        [Fact]
        public void Parse_empty_string_should_throw_argument_null_exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => HttpLink.Parse(""));
            exception.ParamName.ShouldEqual("httpLink");
        }


        [Fact]
        public void Parse_link_that_contains_duplicate_parameters_should_return_link_with_last_parameter_value()
        {
            var link = HttpLink.Parse("<http://nancyfx.org/>; rel=\"up\"; ext=\"first\"; ext=\"second\"");

            link["ext"].ShouldEqual("second");
        }


        [Fact]
        public void Parse_link_that_contains_duplicate_rel_should_return_link_with_last_rel()
        {
            var link = HttpLink.Parse("<http://nancyfx.org/>; rel=\"up\"; rel=\"canonical\"");

            link.Relation.ToString().ShouldEqual("canonical");
        }


        [Fact]
        public void Parse_link_with_extension_parameter_link_object_should_contain_extension_parameter()
        {
            var rel = new HttpLinkRelation("http://nancyfx.org/rels/Nancy");
            var uri = new Uri("http://nancyfx.org/");
            var linkPart = string.Format("<{0}>; rel=\"{1}\"; ext=\"extension-value\"", uri, rel);

            var link = HttpLink.Parse(linkPart);

            link.ShouldNotBeNull();
            link.Relation.ShouldEqual(rel);
            link.TargetUri.ShouldEqual(uri);
            link["ext"].ShouldEqual("extension-value");
        }


        [Fact]
        public void Parse_link_with_only_rel_should_return_link_object()
        {
            var rel = new HttpLinkRelation("http://nancyfx.org/rels/Nancy");
            var uri = new Uri("http://nancyfx.org/");
            var linkPart = string.Format("<{0}>; rel=\"{1}\"", uri, rel);

            var link = HttpLink.Parse(linkPart);

            link.ShouldNotBeNull();
            link.Relation.ShouldEqual(rel);
            link.TargetUri.ShouldEqual(uri);
            link.Title.ShouldBeNull();
        }


        [Fact]
        public void Parse_link_with_title_and_type_should_return_link_object_with_title_and_type()
        {
            var rel = new HttpLinkRelation("http://nancyfx.org/rels/Nancy");
            var uri = new Uri("http://nancyfx.org/");
            const string title = "I'm a link!";
            const string type = "application/json";
            var linkPart = string.Format("<{0}>; rel=\"{1}\"; type=\"{2}\"; title=\"{3}\"", uri, rel, type, title);

            var link = HttpLink.Parse(linkPart);

            link.ShouldNotBeNull();
            link.Relation.ShouldEqual(rel);
            link.TargetUri.ShouldEqual(uri);
            link.Title.ShouldEqual(title);
            link.Type.ToString().ShouldEqual(type);
        }


        [Fact]
        public void Parse_link_with_title_should_return_link_object_with_title()
        {
            var rel = new HttpLinkRelation("http://nancyfx.org/rels/Nancy");
            var uri = new Uri("http://nancyfx.org/");
            const string title = "I'm a link!";
            var linkPart = string.Format("<{0}>; rel=\"{1}\"; title=\"{2}\"", uri, rel, title);

            var link = HttpLink.Parse(linkPart);

            link.ShouldNotBeNull();
            link.Relation.ShouldEqual(rel);
            link.TargetUri.ShouldEqual(uri);
            link.Title.ShouldEqual(title);
        }


        [Fact]
        public void Parse_link_with_type_should_return_link_object_with_type()
        {
            var rel = new HttpLinkRelation("http://nancyfx.org/rels/Nancy");
            var uri = new Uri("http://nancyfx.org/");
            const string type = "application/json";
            var linkPart = string.Format("<{0}>; rel=\"{1}\"; type=\"{2}\"", uri, rel, type);

            var link = HttpLink.Parse(linkPart);

            link.ShouldNotBeNull();
            link.Relation.ShouldEqual(rel);
            link.TargetUri.ShouldEqual(uri);
            link.Type.ShouldNotBeNull();
            link.Type.ToString().ShouldEqual(type);
        }


        [Fact]
        public void Parse_null_should_throw_argument_null_exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => HttpLink.Parse(null));
            exception.ParamName.ShouldEqual("httpLink");
        }


        [Fact]
        public void Parse_space_should_throw_format_exception()
        {
            var exception = Assert.Throws<FormatException>(() => HttpLink.Parse(" "));
            exception.Message.ShouldContain("doesn't contain a target URI");
        }


        [Fact]
        public void ToString_link_with_extension_parameter_should_exist_in_string()
        {
            var link = new HttpLink("http://nancyfx.org/", "up")
            {
                { "ext", "extension-param" }
            };

            link.ToString().ShouldEqual("<http://nancyfx.org/>; rel=\"up\"; ext=\"extension-param\"");
        }
    }
}
