namespace Nancy.ViewEngines.DotLiquid.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Xunit;
    using System.Collections.Generic;

    public class DotLiquidViewEngineFixture
    {
        private readonly DotLiquidViewEngine engine;

        public DotLiquidViewEngineFixture()
        {
            this.engine = new DotLiquidViewEngine(new LiquidNancyFileSystem(""));
        }

        [Fact]
        public void Include_should_look_for_a_partial()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"<h1>Including a partial</h1>{% include 'partial' %}")
            );
            
            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, null);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Including a partial</h1>Some template.");
        }

        [Fact]
        public void Should_support_files_with_the_liquid_extensions()
        {
            var extensions = this.engine.Extensions;

            extensions.ShouldHaveCount(1);
            extensions.ShouldEqual(new[] { "liquid" });
        }

        [Fact]
        public void RenderView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"{% assign name = 'test' %}<h1>Hello Mr. {{ name }}</h1>")
            );

            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, null);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
        
        [Fact]
        public void When_passing_a_null_model_should_return_a_null_model_message_if_called()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"<h1>Hello Mr. {{ model.name }}</h1>")
            );

            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, null);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. [Model is null.]</h1>");
        }
        
        [Fact]
        public void RenderView_should_accept_a_model_and_read_from_it_into_the_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"<h1>Hello Mr. {{ model.name }}</h1>")
            );

            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, new { name= "test" });
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");            
        }
        
        [Fact]
        public void when_calling_a_missing_member_should_return_a_missing_member_message()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"<h1>Hello Mr. {{ model.name }}</h1>")
            );

            var stream = new MemoryStream();

            // When
            var action = this.engine.RenderView(location, new { lastname = "test" });
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. [Can't find :name in the model.]</h1>");
        }

        [Fact]
        public void RenderView_should_accept_a_model_with_a_list_and_iterate_over_it()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                "liquid",
                new StringReader(@"<ul>{% for item in model.Widgets %}<li>{{ item.name }}</li>{% endfor %}</ul>")
            );

            var stream = new MemoryStream();

            // When
            var widgets = new List<object> { new { name = "Widget 1" }, new { name = "Widget 2" }, new { name = "Widget 3" }, new { name = "Widget 4" } };
            var action = this.engine.RenderView(location, new { Widgets = widgets });
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<ul><li>Widget 1</li><li>Widget 2</li><li>Widget 3</li><li>Widget 4</li></ul>"); 
        }
    }

    public class Menu
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IList<MenuItem> Items { get; set; }
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
    public class Article
    {
        public int Id { get; set; }
        public string Body { get; set; }
    }
}
