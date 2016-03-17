namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class HttpLinkHeaderFixture
    {
        private const string LinkHeader = "<http://nancyfx.org/>; "
            + "rel=\"http://nancyfx.org/rel/Nancy\"; "
            + "title=\"Nancy\", "
            + "</alternate.jsonld>; "
            + "rel=\"alternate\"; "
            + "title=\"JSON LD Representation\"";


        [Fact]
        public void Add_existing_relation_in_different_casing_should_overwrite_the_relation()
        {
            var linkHeader = new HttpLinkHeader();
            linkHeader.Add(new HttpLink("/first", "ALTERNATE"));
            linkHeader.Add(new HttpLink("/first", "AlterNate"));
            linkHeader.Add(new HttpLink("/first", "alternate"));
            linkHeader.Add(new HttpLink("/first", HttpLinkRelation.IanaLinkRelationPrefix + "alternate"));

            // TODO: Figure out what to do with duplicates as per https://github.com/mnot/I-D/issues/170. @asbjornu
            var links = linkHeader["alternate"].ToList();
            links.Count.ShouldEqual(1);
            links[0].Relation.ToString().ShouldEqual("alternate");
        }


        [Fact]
        public void Add_existing_relation_in_different_types_should_add_all()
        {
            var linkHeader = new HttpLinkHeader();
            linkHeader.Add(new HttpLink("/first", "ALTERNATE", "application/json"));
            linkHeader.Add(new HttpLink("/second", "AlterNate", "application/xml"));
            linkHeader.Add(new HttpLink("/third", "alternate", "text/html"));

            var links = linkHeader["alternate"].ToList();
            links.Count.ShouldEqual(3);
            links[0].TargetUri.ToString().ShouldEqual("/first");
            links[0].Relation.ToString().ShouldEqual("ALTERNATE");
            links[0].Type.ToString().ShouldEqual("application/json");
            links[1].TargetUri.ToString().ShouldEqual("/second");
            links[1].Relation.ToString().ShouldEqual("AlterNate");
            links[1].Type.ToString().ShouldEqual("application/xml");
            links[2].TargetUri.ToString().ShouldEqual("/third");
            links[2].Relation.ToString().ShouldEqual("alternate");
            links[2].Type.ToString().ShouldEqual("text/html");
        }


        [Fact]
        public void Constructor_should_initialize_collection_with_one_item()
        {
            var linkHeader = new HttpLinkHeader(new HttpLink("/link.json", "alternate"));
            linkHeader.Count.ShouldEqual(1);
        }


        [Fact]
        public void Constructor_should_not_throw_when_links_is_empty()
        {
            var linkHeader = new HttpLinkHeader();
            linkHeader.Count.ShouldEqual(0);
        }


        [Fact]
        public void Constructor_should_not_throw_when_links_is_null()
        {
            var linkHeader = new HttpLinkHeader(null);
            linkHeader.Count.ShouldEqual(0);
        }


        [Fact]
        public void Indexer_should_throw_key_not_found_exception_when_key_is_not_found()
        {
            var linkHeader = HttpLinkHeader.Parse(LinkHeader);
            var key = Guid.NewGuid().ToString();

            var exception = Assert.Throws<KeyNotFoundException>(() => { var l = linkHeader[key]; });
            exception.Message.ShouldContain(key);
        }


        [Fact]
        public void Parse_null_should_throw_argument_null_exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => HttpLinkHeader.Parse(null));
            exception.ParamName.ShouldEqual("linkHeader");
        }


        [Fact]
        public void Parse_valid_link_header_should_return_link_header()
        {
            var linkHeader = HttpLinkHeader.Parse(LinkHeader);

            linkHeader.ShouldNotBeNull();
            linkHeader.Count.ShouldEqual(2);

            var nancyLink = linkHeader["http://nancyfx.org/rel/Nancy"].First();
            var alternateLink = linkHeader["alternate"].First();

            nancyLink.ShouldNotBeNull();
            nancyLink.Title.ShouldEqual("Nancy");
            nancyLink.TargetUri.ToString().ShouldEqual("http://nancyfx.org/");
            nancyLink.Relation.ToString().ShouldEqual("http://nancyfx.org/rel/Nancy");

            alternateLink.ShouldNotBeNull();
            alternateLink.Title.ShouldEqual("JSON LD Representation");
            alternateLink.TargetUri.ToString().ShouldEqual("/alternate.jsonld");
            alternateLink.Relation.ToString().ShouldEqual("alternate");
        }


        [Fact]
        public void ToString_should_return_valid_link_header_string()
        {
            var linkHeader = HttpLinkHeader.Parse(LinkHeader);

            linkHeader.ShouldNotBeNull();
            linkHeader.Count.ShouldEqual(2);

            var linkHeaderString = linkHeader.ToString();

            linkHeaderString.ShouldEqual(LinkHeader);
        }
    }
}
