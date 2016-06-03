namespace Nancy.Tests.Unit
{
    using System;
    using Xunit;

    public class HttpLinkRelationFixture
    {
        [Fact]
        public void Different_relations_should_not_be_equal()
        {
            // Given
            var first = new HttpLinkRelation("alternate");
            var second = new HttpLinkRelation(new Uri("http://nancyfx.org/rels/"), "home");
            var third = new HttpLinkRelation("http://nancyfx.org/rels/nancy");

            // When, Then
            first.ShouldNotEqual(second);
            second.ShouldNotEqual(third);
        }

        [Fact]
        public void Different_relations_should_not_have_equal_hash_code()
        {
            // Given
            var first = new HttpLinkRelation("alternate");
            var second = new HttpLinkRelation(new Uri("http://nancyfx.org/rels/"), "home");
            var third = new HttpLinkRelation("http://nancyfx.org/rels/nancy");

            // When
            var firstHashCode = first.GetHashCode();
            var secondHashCode = second.GetHashCode();
            var thirdHashCode = third.GetHashCode();

            // Then
            firstHashCode.ShouldNotEqual(secondHashCode);
            secondHashCode.ShouldNotEqual(thirdHashCode);
        }

        [Fact]
        public void Iana_relations_with_different_casing_and_prefix_should_all_be_equal()
        {
            // Given
            var first = new HttpLinkRelation("AlterNate");
            var second = new HttpLinkRelation("ALTERNATE");
            var third = new HttpLinkRelation("alternate");
            var fourth = new HttpLinkRelation(HttpLinkRelation.IanaLinkRelationPrefix + "alternate");

            // When, Then
            first.ShouldEqual(second);
            second.ShouldEqual(third);
            third.ShouldEqual(fourth);
        }

        [Fact]
        public void Iana_relations_with_different_casing_and_prefix_should_have_equal_hash_code()
        {
            // Given
            var first = new HttpLinkRelation("AlterNate");
            var second = new HttpLinkRelation("ALTERNATE");
            var third = new HttpLinkRelation("alternate");
            var fourth = new HttpLinkRelation(HttpLinkRelation.IanaLinkRelationPrefix + "alternate");

            // When
            var firstHashCode = first.GetHashCode();
            var secondHashCode = second.GetHashCode();
            var thirdHashCode = third.GetHashCode();
            var fourthHashCode = fourth.GetHashCode();

            // Then
            firstHashCode.ShouldEqual(secondHashCode);
            secondHashCode.ShouldEqual(thirdHashCode);
            thirdHashCode.ShouldEqual(fourthHashCode);
        }

        [Fact]
        public void Parse_null_should_throw_argument_null_exception()
        {
            // Given
            const string relation = null;

            // When
            var exception = Assert.Throws<ArgumentNullException>(() => HttpLinkRelation.Parse(relation));

            // Then
            exception.ParamName.ShouldEqual("relation");
        }

        [Fact]
        public void Parse_relative_value_should_return_iana_prefixed_link_relation()
        {
            // Given
            const string relation = "alternate";

            // When
            var rel = HttpLinkRelation.Parse(relation);

            // Then
            rel.ShouldNotBeNull();
            rel.Prefix.ShouldEqual(HttpLinkRelation.IanaLinkRelationPrefix);
            rel.Value.ShouldEqual(relation);
        }

        [Fact]
        public void Parse_uri_should_return_link_relation()
        {
            // Given
            const string relation = "http://nancyfx.org/rels/something";

            // When
            var rel = HttpLinkRelation.Parse(relation);

            // Then
            rel.ShouldNotBeNull();
            rel.Prefix.ShouldEqual(new Uri("http://nancyfx.org/rels/"));
            rel.Value.ShouldEqual("something");
        }

        [Fact]
        public void Parse_uri_with_path_that_ends_with_slash_throws_format_exception()
        {
            // Given
            const string uri = "http://nancyfx.org/rels/";

            // When
            var exception = Assert.Throws<FormatException>(() => HttpLinkRelation.Parse(uri));

            // Then
            exception.Message.ShouldContain(uri);
        }

        [Fact]
        public void Parse_uri_without_path_throws_format_exception()
        {
            // Given
            const string uri = "http://nancyfx.org/";

            // When
            var exception = Assert.Throws<FormatException>(() => HttpLinkRelation.Parse(uri));

            // Then
            exception.Message.ShouldContain(uri);
        }

        [Fact]
        public void Prefix_and_value_on_iana_relation()
        {
            // Given, When
            var rel = new HttpLinkRelation("alternate");

            // Then
            rel.Prefix.ShouldEqual(HttpLinkRelation.IanaLinkRelationPrefix);
            rel.Value.ShouldEqual("alternate");
        }

        [Fact]
        public void Prefix_and_value_on_non_iana_relation_created_from_prefix_and_value()
        {
            // Given
            var prefix = new Uri("http://nancyfx.org/rels/");

            // When
            var rel = new HttpLinkRelation(prefix, "home");

            // Then
            rel.Prefix.ShouldEqual(prefix);
            rel.Value.ShouldEqual("home");
        }

        [Fact]
        public void Prefix_and_value_on_non_iana_relation_created_from_string()
        {
            // Given
            const string relValue = "http://nancyfx.org/rels/home";

            // When
            var rel = new HttpLinkRelation(relValue);

            // Then
            rel.Prefix.ToString().ShouldEqual("http://nancyfx.org/rels/");
            rel.Value.ShouldEqual("home");
        }

        public void Relation_should_be_equal_to_itself()
        {
            // Given
            var rel = new HttpLinkRelation("alternate");

            // When, Then
            rel.ShouldEqual(rel);
        }
    }
}
