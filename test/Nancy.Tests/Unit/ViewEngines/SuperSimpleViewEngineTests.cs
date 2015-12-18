namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Tests.Fakes;
    using Nancy.ViewEngines.SuperSimpleViewEngine;

    using Xunit;

    public class SuperSimpleViewEngineTests
    {
        private readonly SuperSimpleViewEngine viewEngine;

        private readonly IViewEngineHost fakeHost;

        public SuperSimpleViewEngineTests()
        {
            this.fakeHost = new FakeViewEngineHost();
            this.viewEngine = new SuperSimpleViewEngine(Enumerable.Empty<ISuperSimpleViewEngineMatcher>());
        }

        [Fact]
        public void Should_expand_partial_inside_each_section()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['testing'];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hi";
            var model = new List<string>() { "foo", "bar" };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>HiHi</body></html>", result);
        }

        [Fact]
        public void Should_expand_multiple_partials_inside_each_section()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['greeting'], @Partial['name'];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) =>
            {
                return (s.Equals("greeting")) ? "Hi" : "Nancy";
            };
            var model = new List<string>() { "foo", "bar" };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi, NancyHi, Nancy</body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_inside_each_section_with_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['testing', @Current];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hi @Model ";
            var model = new List<string>() { "foo", "bar" };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi foo Hi bar </body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_inside_each_section_with_parameter_of_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['testing', @Current.Name];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hi @Model ";

            dynamic foo = new ExpandoObject();
            foo.Name = "foo";

            dynamic bar = new ExpandoObject();
            bar.Name = "bar";

            var model = new List<ExpandoObject>() { foo, bar };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi foo Hi bar </body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_inside_each_section_with_property_parameter_of_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['testing', @Current.FirstName];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hi @Model ";

            var clark  = new PersonWithAgeField()
            {
                FirstName = "Clark",
            };

            var lois = new PersonWithAgeField()
            {
                FirstName = "Lois",
            };

            var model = new List<PersonWithAgeField>() { clark, lois };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi Clark Hi Lois </body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_inside_each_section_with_field_parameter_of_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['testing', @Current.Age];@EndEach</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hi @Model ";

            var clark  = new PersonWithAgeField()
            {
                Age = 28,
            };

            var lois = new PersonWithAgeField()
            {
                Age = 37,
            };

            var model = new List<Person>() { clark, lois };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi 28 Hi 37 </body></html>", result);
        }

        [Fact]
        public void Should_expand_multiple_partial_inside_each_section_with_parameter_of_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['first', @Current.Name];-@Partial['second', @Current.Name];@EndEach</body></html>";
            
            var fakeViewEngineHost = new FakeViewEngineHost
            {
                GetTemplateCallback = (s, m) =>
                {
                    return (s.Equals("first")) ?
                        "Hi @Model" :
                        "Hello @Model";
                }
            };

            dynamic foo = new ExpandoObject();
            foo.Name = "foo";

            dynamic bar = new ExpandoObject();
            bar.Name = "bar";

            var model = new List<ExpandoObject>() { foo, bar };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi foo-Hello fooHi bar-Hello bar</body></html>", result);
        }

        [Fact]
        public void Should_expand_multiple_partial_inside_each_section_with_different_parameter_of_current_as_model()
        {
            const string input = @"<html><head></head><body>@Each;@Partial['first', @Current.First];-@Partial['second', @Current.Last];@EndEach</body></html>";

            var fakeViewEngineHost = new FakeViewEngineHost
            {
                GetTemplateCallback = (s, m) =>
                {
                    return (s.Equals("first")) ?
                        "Hi @Model" :
                        "Hello @Model";
                }
            };

            dynamic foo = new ExpandoObject();
            foo.First = "foo";
            foo.Last = "bar";

            dynamic bar = new ExpandoObject();
            bar.First = "baz";
            bar.Last = "bin";

            var model = new List<ExpandoObject>() { foo, bar };

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hi foo-Hello barHi baz-Hello bin</body></html>", result);
        }

        [Fact]
        public void Should_evaluate_current_conditional_inside_each()
        {
            // Given
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current.Name says:@If.IsGreekCitizen;<b>Yay Greece!</b>@EndIf;@IfNot.IsGreekCitizen;<b>Boo Greece!</b>@EndIf;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<object>() { new { Name = "Bob", IsGreekCitizen = true }, new { Name = "Malin", IsGreekCitizen = false } };

            // When
            var output = viewEngine.Render(input, model, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body><ul><li>Bob says:<b>Yay Greece!</b></li><li>Malin says:<b>Boo Greece!</b></li></ul></body></html>", output);
        }

        [Fact]
        public void Should_not_evaluate_current_conditional_from_outside_each()
        {
            // Given
            const string input = @"<html><head></head><body>@If.HasUsers;Yay Users!@EndIf<ul>@Each.Users;<li>@Current.Name:@If.IsGreekCitizen;<b>Yay Greece!</b>@EndIf;</li>@EndEach;</ul>@IfNot.HasUsers;Yay Users!@EndIf</body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<object>() { new { Name = "Bob", IsGreekCitizen = true }, new { Name = "Malin", IsGreekCitizen = false } };

            // When
            var output = viewEngine.Render(input, model, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body>Yay Users!<ul><li>Bob:<b>Yay Greece!</b></li><li>Malin:</li></ul></body></html>", output);
        }


        [Fact]
        public void Should_evaluate_viewbag_as_dynamic_dictionary_conditional()
        {
            const string input = @"@Context.ViewBag.HaveMessage;! @If.Context.ViewBag.HaveMessage;Yay message!@EndIf;";
            var context = new { ViewBag = (dynamic)new DynamicDictionary() };
            context.ViewBag.HaveMessage = true;

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"True! Yay message!", output);
        }

        [Fact]
        public void Should_not_throw_when_viewbag_property_is_null()
        {
            const string input = @"<html><head></head><body>Hey@If.Context.ViewBag.HaveMessage;Yay message!@EndIf;</body></html>";
            var context = new { ViewBag = (dynamic)new DynamicDictionary() };

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hey</body></html>", output);
        }

        [Fact]
        public void Should_replace_primitive_model_with_value()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model</body></html>";
            const string model = "Bob";

            // When
            var output = viewEngine.Render(input, model, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Should_replace_primitive_model_with_value_when_followed_by_closing_tag()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Model;</body></html>";
            const string model = "Bob";

            // When
            var output = viewEngine.Render(input, model, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Should_replaces_valid_property_when_followed_by_closing_tag()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_properties_with_the_same_name()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name;, nice to see you @Model.Name;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob, nice to see you Bob</body></html>", output);
        }

        [Fact]
        public void Should_replace_invalid_properties_with_error_string()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Wrong;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_not_replace_properties_if_case_is_incorrect()
        {
            const string input = @"<html><head></head><body>Hello there @Model.name;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_properties_from_dictionary()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name; - welcome to @Model.SiteName;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";
            model.SiteName = "Cool Site!";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_create_multiple_entries_for_each_statements()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_use_multiple_current_statements_inside_each()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li id=""@Current;"">@Current;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_try_to_use_non_enumerable_in_each_shows_error()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li id=""@Current;"">@Current;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new object();

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul>[ERR!]</ul></body></html>", output);
        }

        [Fact]
        public void Should_combine_single_substitutions_and_each_substitutions()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Nancy";
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_model_statement_to_be_followed_by_a_newline()
        {
            const string input = "<html><head></head><body>Hello there @Model.Name;\n</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal("<html><head></head><body>Hello there Bob\n</body></html>", output);
        }

        [Fact]
        public void Should_allow_each_statements_to_work_over_multiple_lines()
        {
            const string input = "<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>@Each.Users;\n\t\t\t<li>@Current;</li>@EndEach;\n\t\t</ul>\n\t</body>\n</html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal("<html>\n\t<head>\n\t</head>\n\t<body>\n\t\t<ul>\n\t\t\t<li>Bob</li>\n\t\t\t<li>Jim</li>\n\t\t\t<li>Bill</li>\n\t\t</ul>\n\t</body>\n</html>", output);
        }

        [Fact]
        public void Single_substitutions_work_with_standard_anonymous_type_objects()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name; - welcome to @Model.SiteName;</body></html>";
            var model = new { Name = "Bob", SiteName = "Cool Site!" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_allow_each_substitutions_to_work_with_standard_anonymous_type_objects()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li id=""@Current;"">@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li id=""Bob"">Bob</li><li id=""Jim"">Jim</li><li id=""Bill"">Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_substitutions_to_work_with_standard_objects()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_render_block_when_if_statement_returns_true()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_not_render_block_when_if_statement_returns_false()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_not_render_block_when_ifnot_statements_returns_true()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers;<p>No users found!</p>@EndIf;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_render_block_when_ifnot_statement_returns_false()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers;<p>No users found!</p>@EndIf;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><p>No users found!</p><ul></ul></body></html>", output);
        }

        [Fact]
        public void Should_not_conflict_when_if_and_ifNot_statements_combined_but_not_nested()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers;<p>No users found!</p>@EndIf;@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><p>No users found!</p></body></html>", output);
        }

        [Fact]
        public void Should_match_multiple_if_statements_correctly()
        {
            const string input = "@If.One;<p>One</p>@EndIf; @If.Two;<p>Two</p>@EndIf;";
            var model = new { One = true, Two = true };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<p>One</p> <p>Two</p>", output);
        }

        [Fact]
        public void Should_match_correctly_when_multiple_each_statements()
        {
            const string input = "@Each.Users;<li>@Current;</li>@EndEach; @Each.Admins;<li>@Current;</li>@EndEach;";
            var model = new { Users = new List<string> { "1", "2" }, Admins = new List<string> { "3", "4" } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<li>1</li><li>2</li> <li>3</li><li>4</li>", output);
        }

        [Fact]
        public void Should_return_true_for_ifhascollection_when_if_model_has_a_collection_with_items_but_no_bool()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" }, Name = "Nancy" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_return_false_for_ifnot_hascollection_when_model_has_a_collection_with_items_but_no_bool()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers;<p>No Users!</p>@EndIf;</body></html>";
            var model = new { Users = new List<string>() { "Bob", "Jim", "Bill" } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_ignore_item_for_implicit_has_support_when_item_isnt_a_collection()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;<p>Users!</p>@EndIf;</body></html>";
            var model = new { Users = new object() };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_give_precedence_to_hasitem_bool_when_model_has_bool_and_collection()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf;</body></html>";
            var model = new { HasUsers = false, Users = new List<string>() { "Bob", "Jim", "Bill" }, Name = "Nancy" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_allow_property_name_in_current_inside_each_loop_for_dynamic_model_and_dynamic_collection()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current.Name;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            dynamic user1 = new ExpandoObject();
            user1.Name = "Bob";
            dynamic user2 = new ExpandoObject();
            user2.Name = "Jim";
            dynamic user3 = new ExpandoObject();
            user3.Name = "Bill";
            model.Users = new List<object>() { user1, user2, user3 };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_property_name_in_current_inside_each_loop_for_dynamic_model_and_normal_collection()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current.Name;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<User>() { new User("Bob"), new User("Jim"), new User("Bill") };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_property_name_in_current_inside_each_loop_for_normal_model_and_dynamic_collection()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current.Name;</li>@EndEach;</ul></body></html>";
            dynamic user1 = new ExpandoObject();
            user1.Name = "Bob";
            dynamic user2 = new ExpandoObject();
            user2.Name = "Jim";
            dynamic user3 = new ExpandoObject();
            user3.Name = "Bill";
            var model = new { Users = new List<object> { user1, user2, user3 } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_using_model_statement()
        {
            const string input = @"<h1>Hello @Model.User.Name;</h1>";
            var model = new { User = new User("Bob") };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<h1>Hello Bob</h1>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_using_if_statement()
        {
            const string input = @"<h1>Hello @If.User.IsFriend;Friend!@EndIf;</h1>";
            var model = new { User = new User("Bob", true) };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<h1>Hello Friend!</h1>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_using_ifnot_statement()
        {
            const string input = @"<h1>Hello @IfNot.User.IsFriend;Friend!@EndIf;</h1>";
            var model = new { User = new User("Bob", true) };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<h1>Hello </h1>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_for_each_statement()
        {
            const string input = @"<html><head></head><body><ul>@Each.Sub.Users;<li>@Current;</li>@EndEach;</ul></body></html>";
            var model = new { Sub = new { Users = new List<string>() { "Bob", "Jim", "Bill" } } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_for_current_statement_inside_each()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current.Item2.Name;</li>@EndEach;</ul></body></html>";
            var model = new { Users = new List<Tuple<int, User>>() { new Tuple<int, User>(1, new User("Bob")), new Tuple<int, User>(1, new User("Jim")), new Tuple<int, User>(1, new User("Bill")) } };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_Model_substitutions_wihout_semi_colon()
        {
            const string input = @"<html><head></head><body>Hello there @Model.Name</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob</body></html>", output);
        }

        [Fact]
        public void Should_allow_each_without_semi_colon()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users<li>@Current;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_each_and_end_each_without_semi_colon()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users<li>@Current;</li>@EndEach</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_current_within_each_without_semi_colon()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@Current</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            model.Users = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob</li><li>Jim</li><li>Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_if_and_endif_without_semi_colon()
        {
            const string input = @"<html><head></head><body>@If.HasUsers<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul>@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_ifnot_and_endif_without_semi_colon()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers<p>No users found!</p>@EndIf<ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_each_without_a_variable_and_iterate_over_the_model_if_it_is_enumerable()
        {
            const string input = @"<html><head></head><body><ul>@Each<li>Hello @Current</li>@EndEach</ul></body></html>";
            var model = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob</li><li>Hello Jim</li><li>Hello Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_allow_variableless_each_with_semicolon()
        {
            const string input = @"<html><head></head><body><ul>@Each;<li>Hello @Current</li>@EndEach</ul></body></html>";
            var model = new List<string>() { "Bob", "Jim", "Bill" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob</li><li>Hello Jim</li><li>Hello Bill</li></ul></body></html>", output);
        }

        [Fact]
        public void Model_with_exclaimation_should_html_encode()
        {
            const string input = @"<html><head></head><body>Hello there @!Model.Name;</body></html>";
            dynamic model = new ExpandoObject();
            model.Name = "<b>Bob</b>";

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there &lt;b&gt;Bob&lt;/b&gt;</body></html>", output);
        }

        [Fact]
        public void Current_with_exclaimation_and_no_parameters_should_html_encode()
        {
            const string input = @"<html><head></head><body><ul>@Each;<li>Hello @!Current</li>@EndEach</ul></body></html>";
            var model = new List<string>() { "Bob<br/>", "Jim<br/>", "Bill<br/>" };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob&lt;br/&gt;</li><li>Hello Jim&lt;br/&gt;</li><li>Hello Bill&lt;br/&gt;</li></ul></body></html>", output);
        }

        [Fact]
        public void Current_with_explaimation_and_parameters_should_html_encode()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>@!Current.Name;</li>@EndEach;</ul></body></html>";
            dynamic model = new ExpandoObject();
            dynamic user1 = new ExpandoObject();
            user1.Name = "Bob<br/>";
            dynamic user2 = new ExpandoObject();
            user2.Name = "Jim<br/>";
            dynamic user3 = new ExpandoObject();
            user3.Name = "Bill<br/>";
            model.Users = new List<object>() { user1, user2, user3 };

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Bob&lt;br/&gt;</li><li>Jim&lt;br/&gt;</li><li>Bill&lt;br/&gt;</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_expand_basic_partials()
        {
            const string input = @"<html><head></head><body>@Partial['testing'];</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Test partial content";

            var result = viewEngine.Render(input, new object(), fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Test partial content</body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_content_with_model_if_no_model_specified()
        {
            const string input = @"<html><head></head><body>@Partial['testing'];</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hello @Model.Name";
            dynamic model = new ExpandoObject();
            model.Name = "Bob";

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hello Bob</body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_content_with_specified_model_property_if_specified()
        {
            const string input = @"<html><head></head><body>@Partial['testing', Model.User];</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hello @Model.Name";
            dynamic model = new ExpandoObject();
            dynamic subModel = new ExpandoObject();
            model.Name = "Jim";
            subModel.Name = "Bob";
            model.User = subModel;

            var result = viewEngine.Render(input, model, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hello Bob</body></html>", result);
        }

        [Fact]
        public void Should_expand_partial_content_even_with_no_model()
        {
            const string input = @"<html><head></head><body>@Partial['testing'];</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Test partial content";

            var result = this.viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Test partial content</body></html>", result);
        }

        [Fact]
        public void Should_try_to_locate_master_page_if_one_specified()
        {
            const string input = "@Master['myMaster']\r\n@Section['Header'];\r\nHeader\r\n@EndSection\r\n@Section['Footer']\r\nFooter\r\n@EndSection";
            var called = false;
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) =>
            {
                called = (s == "myMaster");
                return "";
            };

            this.viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.True(called);
        }

        [Fact]
        public void Should_replace_sections_in_master_page()
        {
            const string input = "@Master['myMaster']\r\n@Section['Header'];\r\nHeader\r\n@EndSection\r\n@Section['Footer']\r\nFooter\r\n@EndSection";
            const string master = @"<div id='header'>@Section['Header'];</div><div id='footer'>@Section['Footer'];</div>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => master;

            var result = this.viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal("<div id='header'>\r\nHeader\r\n</div><div id='footer'>\r\nFooter\r\n</div>", result);
        }

        [Fact]
        public void Should_also_expand_master_page_with_same_model()
        {
            const string input = "@Master['myMaster']\r\n@Section['Header'];\r\nHeader\r\n@EndSection\r\n@Section['Footer']\r\nFooter\r\n@EndSection";
            const string master = @"Hello @Model.Name!<div id='header'>@Section['Header'];</div><div id='footer'>@Section['Footer'];</div>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => master;

            var result = viewEngine.Render(input, new { Name = "Bob" }, fakeViewEngineHost);

            Assert.Equal("Hello Bob!<div id='header'>\r\nHeader\r\n</div><div id='footer'>\r\nFooter\r\n</div>", result);
        }

        [Fact]
        public void Should_handle_master_page_hierarchies()
        {
            const string input = "@Master['middle']\r\n@Section['MiddleContent']Middle@EndSection";
            const string middle = "@Master['top']\r\n@Section['TopContent']Top\r\n@Section['MiddleContent']@EndSection";
            const string top = "Top! @Section['TopContent']";

            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => s == "middle" ? middle : top;

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal("Top! Top\r\nMiddle", result);
        }

        [Fact]
        public void Should_call_to_expand_paths()
        {
            const string input = @"<script src='@Path['~/scripts/test.js']'></script>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.ExpandPathCallBack = s => s.Replace("~/", "/BasePath/");

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal("<script src='/BasePath/scripts/test.js'></script>", result);
        }

        [Fact]
        public void Should_expand_paths_in_attribute_values()
        {
            const string input = @"<script src='~/scripts/test.js'></script> <link href=""~/stylesheets/style.css"" />";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.ExpandPathCallBack = s => s.Replace("~/", "/BasePath/");

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal(@"<script src='/BasePath/scripts/test.js'></script> <link href=""/BasePath/stylesheets/style.css"" />", result);
        }


        [Fact]
        public void Should_expand_anti_forgery_tokens()
        {
            const string input = "<html><body><form>@AntiForgeryToken</form><body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal("<html><body><form>CSRF</form><body></html>", result);
        }

        [Fact]
        public void Should_replace_primitive_context_with_value()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Context</body></html>";

            ((FakeViewEngineHost)this.fakeHost).Context = "Frank";

            // When
            var output = viewEngine.Render(input, null, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Frank</body></html>", output);
        }

        [Fact]
        public void Should_replace_primitive_context_with_value_when_followed_by_closing_tag()
        {
            // Given
            const string input = @"<html><head></head><body>Hello there @Context;</body></html>";
            ((FakeViewEngineHost)this.fakeHost).Context = "Frank";

            // When
            var output = viewEngine.Render(input, null, this.fakeHost);

            // Then
            Assert.Equal(@"<html><head></head><body>Hello there Frank</body></html>", output);
        }

        [Fact]
        public void Should_replaces_valid_context_property_when_followed_by_closing_tag()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Name;</body></html>";
            dynamic context = new ExpandoObject();
            context.Name = "Frank";

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Frank</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_context_properties_with_the_same_name()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Name;, nice to see you @Context.Name;</body></html>";
            dynamic context = new ExpandoObject();
            context.Name = "Frank";

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Frank, nice to see you Frank</body></html>", output);
        }

        [Fact]
        public void Should_replace_invalid_context_properties_with_error_string()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Wrong;</body></html>";
            dynamic context = new ExpandoObject();
            context.Name = "Frank";

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_not_replace_context_properties_if_case_is_incorrect()
        {
            const string input = @"<html><head></head><body>Hello there @Context.name;</body></html>";
            dynamic context = new ExpandoObject();
            context.Name = "Franke";

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there [ERR!]</body></html>", output);
        }

        [Fact]
        public void Should_replace_multiple_context_properties_from_dictionary()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Name; - welcome to @Context.SiteName;</body></html>";
            dynamic context = new ExpandoObject();
            context.Name = "Frank";
            context.SiteName = "Cool Site!";

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Frank - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_allow_context_statement_to_be_followed_by_a_newline()
        {
            const string input = "<html><head></head><body>Hello there @Context.Name;\n</body></html>";

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal("<html><head></head><body>Hello there Frank\n</body></html>", output);
        }

        [Fact]
        public void Context_substitutions_work_with_standard_anonymous_type_objects()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Name; - welcome to @Context.SiteName;</body></html>";
            var context = new { Name = "Bob", SiteName = "Cool Site!" };

            ((FakeViewEngineHost)this.fakeHost).Context = context;

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Bob - welcome to Cool Site!</body></html>", output);
        }

        [Fact]
        public void Should_allow_sub_properties_using_context_statement()
        {
            const string input = @"<h1>Hello @Context.User.Username;</h1>";

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<h1>Hello Frank123</h1>", output);
        }

        [Fact]
        public void Should_allow_Context_substitutions_wihout_semi_colon()
        {
            const string input = @"<html><head></head><body>Hello there @Context.Name</body></html>";

            var output = viewEngine.Render(input, null, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello there Frank</body></html>", output);
        }

        [Fact]
        public void Should_expand_partial_content_with_context()
        {
            const string input = @"<html><head></head><body>@Partial['testing'];</body></html>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => "Hello @Context.Name";

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal(@"<html><head></head><body>Hello Frank</body></html>", result);
        }

        [Fact]
        public void Should_also_expand_master_page_with_same_context()
        {
            const string input = "@Master['myMaster']\r\n@Section['Header'];\r\nHeader\r\n@EndSection\r\n@Section['Footer']\r\nFooter\r\n@EndSection";
            const string master = @"Hello @Context.Name!<div id='header'>@Section['Header'];</div><div id='footer'>@Section['Footer'];</div>";
            var fakeViewEngineHost = new FakeViewEngineHost();
            fakeViewEngineHost.GetTemplateCallback = (s, m) => master;

            var result = viewEngine.Render(input, null, fakeViewEngineHost);

            Assert.Equal("Hello Frank!<div id='header'>\r\nHeader\r\n</div><div id='footer'>\r\nFooter\r\n</div>", result);
        }


        [Fact]
        public void Should_stuffrender_block_when_ifnot_statement_returns_false()
        {
            const string input = @"<html><head></head><body>@IfNot.Context.HasUsers;<p>No users found!</p>@EndIf;</body></html>";

            var model = new FakeModel("Nancy", new List<string>() { "Nancy " });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><p>No users found!</p></body></html>", output);
        }

        [Fact]
        public void Should_allow_ifnot_and_endif_and_context_model_source()
        {
            const string input = @"<html><head></head><body>@IfNot.Context.HasUsers;No users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>No users found!</body></html>", output);
        }

        [Fact]
        public void Should_allow_ifnot_and_endif_and_model_model_source()
        {
            const string input = @"<html><head></head><body>@IfNot.Model.HasUsers;No users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>No users found!</body></html>", output);
        }

        [Fact]
        public void Should_allow_ifnot_and_endif_and_implicit_model_model_source()
        {
            const string input = @"<html><head></head><body>@IfNot.HasUsers;No users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>No users found!</body></html>", output);
        }

        [Fact]
        public void Should_allow_if_and_endif_and_context_model_source()
        {
            const string input = @"<html><head></head><body>@If.Context.HasUsers;Users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>());

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>() { "Bob", "Jim", "Bill" });

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Users found!</body></html>", output);
        }

        [Fact]
        public void Should_allow_if_and_endif_and_model_model_source()
        {
            const string input = @"<html><head></head><body>@If.Model.HasUsers;Users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Users found!</body></html>", output);
        }

        [Fact]
        public void Should_allow_if_and_endif_and_implicit_model_model_source()
        {
            const string input = @"<html><head></head><body>@If.HasUsers;Users found!@EndIf</body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Users found!</body></html>", output);
        }

        [Fact]
        public void Should_support_each_block_with_model_as_model_source()
        {
            const string input = @"<html><head></head><body><ul>@Each.Model.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_support_each_block_with_implicit_model_source()
        {
            const string input = @"<html><head></head><body><ul>@Each.Users;<li>Hello @Current;, @Model.Name; says hello!</li>@EndEach;</ul></body></html>";
            var model = new FakeModel("Nancy", new List<string>() { "Bob", "Jim", "Bill" });

            ((FakeViewEngineHost)this.fakeHost).Context = new FakeModel("NancyContext", new List<string>());

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body><ul><li>Hello Bob, Nancy says hello!</li><li>Hello Jim, Nancy says hello!</li><li>Hello Bill, Nancy says hello!</li></ul></body></html>", output);
        }

        [Fact]
        public void Should_include_block_with_ifnull_if_value_null()
        {
            const string input = @"<html><head></head><body>@IfNull.Name;No users found@EndIf;</body></html>";
            var model = new User(null, true);

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>No users found</body></html>", output);
        }

        [Fact]
        public void Should_not_include_block_with_ifnull_if_value_non_null()
        {
            const string input = @"<html><head></head><body>@IfNull.Name;No users found@EndIf;</body></html>";
            var model = new User("Bob", true);

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_include_block_with_ifnotnull_if_value_non_null()
        {
            const string input = @"<html><head></head><body>@IfNotNull.Name;Hello @Model.Name@EndIf;</body></html>";
            var model = new User("Bob", true);

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body>Hello Bob</body></html>", output);
        }

        [Fact]
        public void Should_not_include_block_with_ifnotnull_if_value_null()
        {
            const string input = @"<html><head></head><body>@IfNotNull.Name;Hello @Model.Name@EndIf;</body></html>";
            var model = new User(null, true);

            var output = viewEngine.Render(input, model, this.fakeHost);

            Assert.Equal(@"<html><head></head><body></body></html>", output);
        }

        [Fact]
        public void Should_invoke_all_matchers()
        {
            // Given
            var extension1 = A.Fake<ISuperSimpleViewEngineMatcher>();
            A.CallTo(() => extension1.Invoke(A<string>._, A<object>._, A<IViewEngineHost>._)).Returns("Foo");

            var extension2 = A.Fake<ISuperSimpleViewEngineMatcher>();
            A.CallTo(() => extension2.Invoke(A<string>._, A<object>._, A<IViewEngineHost>._)).ReturnsLazily(x => (string)x.Arguments[0] + "Bar");

            const string input = @"<html><head></head><body></body></html>";

            var engine = new SuperSimpleViewEngine(new[] { extension1, extension2 });

            // When
            var output = engine.Render(input, null, this.fakeHost);

            // Then
            Assert.Equal("FooBar", output);
        }

        [Fact]
        public void Should_support_being_passed_null_matchers()
        {
            // Given
            const string input = @"<html><head></head><body></body></html>";

            var engine = new SuperSimpleViewEngine(null);

            // When
            var output = engine.Render(input, null, this.fakeHost);

            // Then
            Assert.Equal(input, output);
        }

        [Fact]
        public void Shoud_pass_model_to_matchers()
        {
            // Given
            const string input = @"<html><head></head><body></body></html>";

            var extension = A.Fake<ISuperSimpleViewEngineMatcher>();
            var engine = new SuperSimpleViewEngine(new[] { extension });
            var model = new object();

            // When
            engine.Render(input, model, this.fakeHost);

            // Then
            A.CallTo(() => extension.Invoke(A<string>._, model, A<IViewEngineHost>._)).MustHaveHappened();
        }

        [Fact]
        public void Shoud_pass_content_to_matchers()
        {
            // Given
            const string input = @"<html><head></head><body></body></html>";

            var extension = A.Fake<ISuperSimpleViewEngineMatcher>();
            var engine = new SuperSimpleViewEngine(new[] { extension });

            // When
            engine.Render(input, null, this.fakeHost);

            // Then
            A.CallTo(() => extension.Invoke(input, A<object>._, A<IViewEngineHost>._)).MustHaveHappened();
        }

        [Fact]
        public void Shoud_pass_view_engine_host_to_matchers()
        {
            // Given
            const string input = @"<html><head></head><body></body></html>";

            var extension = A.Fake<ISuperSimpleViewEngineMatcher>();
            var engine = new SuperSimpleViewEngine(new[] { extension });

            // When
            engine.Render(input, null, this.fakeHost);

            // Then
            A.CallTo(() => extension.Invoke(A<string>._, A<object>._, this.fakeHost)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_context_nested_dictionary_to_matchers()
        {
            // Given
            const string keyConst = "TestKey";
            const string valueConst = "testvalue";

            var dict = new Dictionary<string, object> { { keyConst, valueConst } };
            var context = this.fakeHost.Context as FakeViewEngineHost.FakeContext;
            context.ViewBag.Nested = new
            {
                Dictionary = dict
            };

            var input = string.Format(@"<html><head></head><body>@Context.ViewBag.Nested.Dictionary.{0}</body></html>", keyConst);
            var engine = new SuperSimpleViewEngine();

            // When
            var result = engine.Render(input, null, this.fakeHost);

            // Then
            Assert.Equal(string.Format("<html><head></head><body>{0}</body></html>", valueConst), result);
        }
    }

    public class User
    {
        public User(string name)
            : this(name, false)
        {
        }

        public User(string name, bool isFriend)
        {
            Name = name;
            IsFriend = isFriend;
        }

        public string Name { get; private set; }
        public bool IsFriend { get; private set; }
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