namespace Nancy.ViewEngines.Spark.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using FakeItEasy;
    using global::Spark.Parser;
    using Nancy.Tests;
    using Nancy.ViewEngines.Spark.Descriptors;
    using Spark;
    using Xunit;
    using global::Spark;
    using global::Spark.FileSystem;
    using SparkViewEngine = Spark.SparkViewEngine;

    public class DescriptorBuildingFixture
    {
        private readonly ActionContext actionContext;
        private readonly SparkViewEngine engine;
        private readonly InMemoryViewFolder viewFolder;
        private readonly IRootPathProvider rootPathProvider;

        public DescriptorBuildingFixture()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            this.viewFolder = new InMemoryViewFolder();
            this.engine = new SparkViewEngine(this.rootPathProvider) {ViewFolder = viewFolder};
            this.actionContext = new ActionContext(A.Fake<HttpContextBase>(), "Bar");
        }
        
        [Fact]
        public void Build_descriptor_extra_params_set_to_null_should_acts_as_empty()
        {
            //Given, When
            var param1 = new BuildDescriptorParams("foo", "fizz", "buzz", false, null);
            var param2 = new BuildDescriptorParams("foo", "fizz", "buzz", false, new Dictionary<string, object>());

            //Then
            param1.ShouldEqual(param2);
        }

        [Fact]
        public void Build_descriptor_extra_params_should_have_identical_equality()
        {
            //Given, When
            var param1 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new[] { "hippo", "lion" }));
            var param2 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new[] {"hippo"}));
            var param3 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new[] {"lion"}));
            var param4 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new[] {"lion", "hippo"}));
            var param5 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(null));
            var param6 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new string[0]));
            var param7 = new BuildDescriptorParams("foo", "fizz", "buzz", false, Dict(new[] {"hippo", "lion"}));

            //Then
            param1.ShouldNotEqual(param2);
            param1.ShouldNotEqual(param3);
            param1.ShouldNotEqual(param4);
            param1.ShouldNotEqual(param5);
            param1.ShouldNotEqual(param6);
            param1.ShouldEqual(param7);
        }

        [Fact]
        public void Build_descriptor_params_should_act_as_a_unique_key()
        {
            //Given, When
            var param1 = new BuildDescriptorParams("foo", "fizz", "buzz", false, null);
            var param2 = new BuildDescriptorParams("foo", "bar", "buzz", false, null);
            var param3 = new BuildDescriptorParams("foo", "fizz", "buzz", true, null);

            //Then
            param1.ShouldNotEqual(param2);
            param1.ShouldNotEqual(param3);
        }

        [Fact]
        public void Custom_descriptor_builders_should_not_be_able_to_use_descriptor_filters()
        {
            //Given
            engine.DescriptorBuilder = A.Fake<IDescriptorBuilder>();

            //When
            var exception = Record.Exception(() => engine.AddFilter(A.Fake<IDescriptorFilter>()));

            //Then
            exception.ShouldBeOfType<InvalidCastException>();
        }

        [Fact]
        public void Descriptors_with_custom_parameter_should_be_added_to_the_view_search_path()
        {
            //Given
            engine.DescriptorBuilder = new ExtendedDescriptorBuilder(engine.Engine);
            actionContext.ExtraData.Add("language", "en-gb");
            viewFolder.Add(@"Bar\Index.en-gb.spark", "");
            viewFolder.Add(@"Bar\Index.en.spark", "");
            viewFolder.Add(@"Bar\Index.spark", "");
            viewFolder.Add(@"Layouts\Application.en.spark", "");
            viewFolder.Add(@"Layouts\Application.it.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", null, true, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.en-gb.spark", @"Layouts\Application.en.spark");
        }

        [Fact]
        public void Descriptors_with_named_master_should_override_the_view_master()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            viewFolder.Add(@"Layouts\Whale.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");
            viewFolder.Add(@"Layouts\Bar.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", "Elephant", true, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Elephant.spark", @"Layouts\Whale.spark");
        }

        [Fact]
        public void Descriptors_with_normal_view_and_default_layout_present()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", null, true, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Application.spark");
        }

        [Fact]
        public void Descriptors_with_normal_view_and_named_master()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");
            viewFolder.Add(@"Layouts\Home.spark", "");
            viewFolder.Add(@"Layouts\Site.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", "Site", true, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Site.spark");
        }

        [Fact]
        public void Descriptors_with_normal_view_and_no_default_layout()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", null, true, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark");
        }

        [Fact]
        public void Descriptors_with_partial_view_should_ignore_default_layouts()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");
            viewFolder.Add(@"Layouts\Home.spark", "");
            viewFolder.Add(@"Shared\Application.spark", "");
            viewFolder.Add(@"Shared\Home.spark", "");

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", null, false, new List<string>());

            //Then
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark");
        }

        [Fact]
        public void Descriptors_with_partial_view_should_ignore_use_master_and_default()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            viewFolder.Add(@"Layouts\Whale.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");
            viewFolder.Add(@"Layouts\Bar.spark", "");

            //When
            var searchedLocations = new List<string>();
            var result = engine.CreateDescriptor(actionContext, "Index", null, false, searchedLocations);

            //Then
            AssertDescriptorTemplates(result, searchedLocations, @"Bar\Index.spark");
        }

        [Fact]
        public void Descriptors_with_simplified_use_master_grammar_should_detect_element_correctly()
        {
            //Given
            var builder = new DefaultDescriptorBuilder();

            //When
            var a = builder.ParseUseMaster(new Position(new SourceContext("<use master='a'/>")));
            var b = builder.ParseUseMaster(new Position(new SourceContext("<use\r\nmaster \r\n =\r\n'b' />")));
            var c = builder.ParseUseMaster(new Position(new SourceContext("<use master=\"c\"/>")));
            var def = builder.ParseUseMaster(new Position(new SourceContext("  x <use etc=''/> <use master=\"def\"/> y ")));
            var none = builder.ParseUseMaster(new Position(new SourceContext("  x <use etc=''/> <using master=\"def\"/> y ")));
            var g = builder.ParseUseMaster(new Position(new SourceContext("-<use master=\"g\"/>-<use master=\"h\"/>-")));

            //Then
            a.Value.ShouldEqual("a");
            b.Value.ShouldEqual("b");
            c.Value.ShouldEqual("c");
            def.Value.ShouldEqual("def");
            none.ShouldBeNull();
            g.Value.ShouldEqual("g");
        }

        [Fact]
        public void Descriptors_with_use_master_should_create_a_template_chain()
        {
            //Given
            viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            viewFolder.Add(@"Layouts\Whale.spark", "");
            viewFolder.Add(@"Layouts\Application.spark", "");
            viewFolder.Add(@"Layouts\Bar.spark", "");
            var searchedLocations = new List<string>();

            //When
            var result = engine.CreateDescriptor(actionContext, "Index", null, true, searchedLocations);

            //Then
            AssertDescriptorTemplates(result, searchedLocations, @"Bar\Index.spark", @"Layouts\Lion.spark", @"Layouts\Elephant.spark", @"Layouts\Whale.spark");
        }

        private static void AssertDescriptorTemplates(SparkViewDescriptor descriptor, IEnumerable<string> searchedLocations, params string[] templates)
        {
            templates.ShouldHaveCount(descriptor.Templates.Count);
            for (var index = 0; index != templates.Length; ++index)
            {
                templates[index].ShouldEqual(descriptor.Templates[index]);
            }
            searchedLocations.ShouldHaveCount(0);
        }

        private static IDictionary<string, object> Dict(IEnumerable<string> values)
        {
            return (values == null) ? null : values.Select((v, k) => new { k, v }).ToDictionary(kv => kv.k.ToString(), kv => (object)kv.v);
        }
    }

    internal class ExtendedDescriptorBuilder : DefaultDescriptorBuilder
    {
        public ExtendedDescriptorBuilder(ISparkViewEngine engine)
            : base(engine)
        {
        }

        public override IDictionary<string, object> GetExtraParameters(ActionContext actionContext)
        {
            return new Dictionary<string, object> { {"language", Convert.ToString(actionContext.ExtraData["language"])} };
        }

        protected override IEnumerable<string> PotentialViewLocations(string actionName, string viewName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialViewLocations(actionName, viewName, extra), extra["language"].ToString());
        }

        protected override IEnumerable<string> PotentialMasterLocations(string masterName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialMasterLocations(masterName, extra), extra["language"].ToString());
        }

        protected override IEnumerable<string> PotentialDefaultMasterLocations(string actionName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialDefaultMasterLocations(actionName, extra), extra["language"].ToString());
        }

        private static IEnumerable<string> Merge(IEnumerable<string> locations, string region)
        {
            var slashPos = (region ?? "").IndexOf('-');

            if (region != null)
            {
                var language = slashPos == -1 ? null : region.Substring(0, slashPos);

                foreach (var location in locations)
                {
                    if (!string.IsNullOrEmpty(region))
                    {
                        yield return Path.ChangeExtension(location, region + ".spark");
                        if (slashPos != -1)
                        {
                            yield return Path.ChangeExtension(location, language + ".spark");
                        }
                    }
                    yield return location;
                }
            }
        }
    }
}