namespace Nancy.Tests.Unit.ViewEngines
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Nancy.ViewEngines;
    using Xunit;

    public class SuperSimpleViewEngineTests
    {
        private readonly SuperSimpleViewEngine viewEngine;

        public SuperSimpleViewEngineTests()
        {
            this.viewEngine = new SuperSimpleViewEngine();
        }

        [Fact]
        public void Should_not_throw_exception_when_model_it_null()
        {
            // Given
            const string input = @"<html><head></head><body>Hello</body></html>";
            dynamic model = null;

            // When
            var exception =
                Record.Exception(() => this.viewEngine.Render(input, model));

            // Then
            exception.ShouldBeNull();
        }

        [Fact]
        public void Should_replaces_valid_property_when_followed_by_closing_tag()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.Name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_properties_with_the_same_name()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.Name, nice to see you @Model.Name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob, nice to see you Bob</body></html>", output);
        }

        [Fact]
        public void Should_replace_invalid_properties_with_error_string()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.Wrong</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_not_replace_properties_if_case_is_incorrect()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            // When
            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_properties_from_dictionary()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.Name - welcome to @Model.SiteName</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";
            model.SiteName = "Cool Site!";

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_create_multiple_entries_for_each_statements()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li>@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_use_multiple_current_statements_inside_each()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_try_to_use_non_enumerable_in_each_shows_error()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new object();

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul>[ERR!]</ul></body></html>", output);
        }

        [Fact]
        public void Should_combine_single_substitutions_and_each_substitutions()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Nancy";
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_model_statement_to_be_followed_by_a_newline()
        {
            // Given
            const string input = "<html><head></head><body>Hello there @Model.Name\n</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal("<html><head></head><body>Hello there Bob\n</body></html>", output);
        }

        [Fact]
        public void Should_allow_each_statements_to_work_over_multiple_lines()
        {
            // Given
            const string input = "<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>@Each.Users\n\t\t\t<li>@Current</li>@EndEach\n\t\t</ul>\n\t</body>\n</html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal("<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>\n\t\t\t<li>Bob</li>\n\t\t\t<li>Jim</li>\n\t\t\t<li>Bill</li>\n\t\t</ul>\n\t</body>\n</html>", output);
        }

        [Fact]
        public void Single_substitutions_work_with_standard_anonymous_type_objects()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model.Name - welcome to @Model.SiteName</body></html>";
            var model = new { Name = "Bob", SiteName = "Cool Site!" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_allow_each_substitutions_to_work_with_standard_anonymous_type_objects()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_substitutions_to_work_with_standard_objects()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_render_block_when_if_statement_returns_true()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_not_render_block_when_if_statement_returns_false()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_not_render_block_when_ifnot_statements_returns_true()
        {
            // Given
            const string input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_render_block_when_ifnot_statement_returns_false()
        {
            // Given
            const string input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><p>No users found!</p><ul></ul></body></html>", output);
        }

        [Fact]
        public void Should_not_conflict_when_if_and_ifNot_statements_combined_but_not_nested()
        {
            // Given
            const string input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><p>No users found!</p></body></html>", output);
        }

        [Fact]
        public void Should_match_multiple_if_statements_correctly()
        {
            // Given
            const string input = "@If.One<p>One</p>@EndIf @If.Two<p>Two</p>@EndIf";
            var model = new { One = true, Two = true };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<p>One</p> <p>Two</p>", output);
        }

        [Fact]
        public void Should_match_correctly_when_multiple_each_statements()
        {
            // Given
            const string input = "@Each.Users<li>@Current</li>@EndEach @Each.Admins<li>@Current</li>@EndEach";
            var model = new { Users = new List<string> { "1", "2" }, Admins = new List<string> { "3", "4" } };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<li>1</li><li>2</li> <li>3</li><li>4</li>", output);
        }

        [Fact]
        public void Should_return_true_for_ifhascollection_when_if_model_has_a_collection_with_items_but_no_bool()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" }, Name = "Nancy" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_return_false_for_ifnot_hascollection_when_model_has_a_collection_with_items_but_no_bool()
        {
            // Given
            const string input = @"<html><head></head><body>@IfNot.HasUsers<p>No Users!</p>@EndIf</body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_ignore_item_for_implicit_has_support_when_item_isnt_a_collection()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers<p>Users!</p>@EndIf</body></html>";
            var model = new { Users = new object() };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_give_precedence_to_hasitem_bool_when_model_has_bool_and_collection()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new { HasUsers = false, Users = new List<string>() { "Bob", "Jim", "Bill" }, Name = "Nancy" };

            // When
            var output = viewEngine.Render(input, model);

            // Then
            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }
    }

    public class FakeModel
    {
        public FakeModel(string name, List<string> users)
        {
            this.Name = name;
            this.Users = users;
        }

        public List<string> Users { get; private set; }

        public string Name { get; private set; }

        public bool HasUsers
        {
            get
            {
                return this.Users.Any();
            }
        }
    }
}