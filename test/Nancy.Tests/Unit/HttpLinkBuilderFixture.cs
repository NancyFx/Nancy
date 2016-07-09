namespace Nancy.Tests.Unit
{
    using Xunit;

    public class HttpLinkBuilderFixture
    {
        [Fact]
        public void Add_existing_relation_in_different_types_should_add_all()
        {
            // Given
            var linkBuilder = new HttpLinkBuilder();

            // When
            linkBuilder.Add(new HttpLink("/first", "ALTERNATE", "application/json"));
            linkBuilder.Add(new HttpLink("/second", "AlterNate", "application/xml"));
            linkBuilder.Add(new HttpLink("/third", "alternate", "text/html"));

            // Then
            linkBuilder.Count.ShouldEqual(3);
            linkBuilder[0].TargetUri.ToString().ShouldEqual("/first");
            linkBuilder[0].Relation.ToString().ShouldEqual("ALTERNATE");
            linkBuilder[0].Type.ToString().ShouldEqual("application/json");
            linkBuilder[1].TargetUri.ToString().ShouldEqual("/second");
            linkBuilder[1].Relation.ToString().ShouldEqual("AlterNate");
            linkBuilder[1].Type.ToString().ShouldEqual("application/xml");
            linkBuilder[2].TargetUri.ToString().ShouldEqual("/third");
            linkBuilder[2].Relation.ToString().ShouldEqual("alternate");
            linkBuilder[2].Type.ToString().ShouldEqual("text/html");
        }

        [Fact]
        public void Count_should_be_zero_after_instantiation()
        {
            // Given, When
            var linkHeader = new HttpLinkBuilder();

            // Then
            linkHeader.Count.ShouldEqual(0);
        }

        [Fact]
        public void Count_should_equal_four_after_adding_four_links_with_same_relation_in_different_casing()
        {
            // Given
            var linkBuilder = new HttpLinkBuilder();

            // When
            linkBuilder.Add(new HttpLink("/first", "ALTERNATE"));
            linkBuilder.Add(new HttpLink("/first", "AlterNate"));
            linkBuilder.Add(new HttpLink("/first", "alternate"));
            linkBuilder.Add(new HttpLink("/first", HttpLinkRelation.IanaLinkRelationPrefix + "alternate"));

            // Then
            linkBuilder.Count.ShouldEqual(4);
        }
    }
}
