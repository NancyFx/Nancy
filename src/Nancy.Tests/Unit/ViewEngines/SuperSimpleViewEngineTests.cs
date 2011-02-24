namespace Nancy.Tests.Unit.ViewEngines
{
    using System.Dynamic;
    using System.Linq;
    using Nancy.ViewEngines;
    using Xunit;

    using System.Collections.Generic;

    public class SuperSimpleViewEngineTests
    {
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

        private SuperSimpleViewEngine viewEngine;

        public SuperSimpleViewEngineTests()
        {
            this.viewEngine = new SuperSimpleViewEngine();
        }

        [Fact]
        public void Replaces_valid_property_when_followed_by_closing_tag()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Replaces_multiple_properties_with_the_same_name()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name, nice to see you @Model.Name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there Bob, nice to see you Bob</body></html>", output);
        }

        [Fact]
        public void Replaces_invalid_properties_with_error_string()
        {
            var input = @"<html><head></head><body>Hello there @Model.Wrong</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Does_not_replace_properties_if_case_is_incorrect()
        {
            var input = @"<html><head></head><body>Hello there @Model.name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Replaces_multiple_properties_from_dictionary()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name - welcome to @Model.SiteName</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";
            model.SiteName = "Cool Site!";

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Creates_multiple_entries_for_each_statements()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li>@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Can_use_multiple_current_statements_inside_each()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Trying_to_use_non_enumerable_in_each_shows_error()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new object();

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul>[ERR!]</ul></body></html>", output);
        }

        [Fact]
        public void Can_combine_single_substitutions_and_each_substitutions()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Nancy";
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Model_statement_can_be_followed_by_a_newline()
        {
            var input = "<html><head></head><body>Hello there @Model.Name\n</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model);

            Assert.Equal("<html><head></head><body>Hello there Bob\n</body></html>", output);
        }

        [Fact]
        public void Each_statements_should_work_over_multiple_lines()
        {
            var input = "<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>@Each.Users\n\t\t\t<li>@Current</li>@EndEach\n\t\t</ul>\n\t</body>\n</html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model);

            Assert.Equal("<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>\n\t\t\t<li>Bob</li>\n\t\t\t<li>Jim</li>\n\t\t\t<li>Bill</li>\n\t\t</ul>\n\t</body>\n</html>", output);
        }

        [Fact]
        public void Single_substitutions_work_with_standard_anonymous_type_objects()
        {
            var input = @"<html><head></head><body>Hello there @Model.Name - welcome to @Model.SiteName</body></html>";
            var model = new { Name = "Bob", SiteName = "Cool Site!" };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Each_substitutions_work_with_standard_anonymous_type_objects()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li id=""@Current"">@Current</li>@EndEach</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Substitutions_work_with_standard_objects()
        {
            var input = @"<html><head></head><body><ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void If_statement_with_true_returned_renders_block()
        {
            var input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void If_statement_with_false_returned_does_not_render_block()
        {
            var input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void IfNot_statement_with_true_returned_does_not_renders_block()
        {
            var input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void IfNot_statement_with_false_returned_renders_block()
        {
            var input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><p>No users found!</p><ul></ul></body></html>", output);
        }

        [Fact]
        public void If_and_IfNot_statements_combined_but_not_nested_do_not_conflict()
        {
            var input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf@If.HasUsers<ul>@Each.Users<li>Hello @Current, @Model.Name says hello!</li>@EndEach</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<html><head></head><body><p>No users found!</p></body></html>", output);
        }

        [Fact]
        public void Multiple_if_statements_match_correctly()
        {
            var input = "@If.One<p>One</p>@EndIf @If.Two<p>Two</p>@EndIf";
            var model = new { One = true, Two = true };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<p>One</p> <p>Two</p>", output);
        }

        [Fact]
        public void Multiple_each_statements_match_correctly()
        {
            var input = "@Each.Users<li>@Current</li>@EndEach @Each.Admins<li>@Current</li>@EndEach";
            var model = new { Users = new List<string> { "1", "2" }, Admins = new List<string> { "3", "4" } };

            var output = viewEngine.Render(input, model);

            Assert.Equal(@"<li>1</li><li>2</li> <li>3</li><li>4</li>", output);
        }
    }
}
