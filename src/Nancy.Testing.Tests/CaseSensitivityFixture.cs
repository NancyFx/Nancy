namespace Nancy.Testing.Tests
{
    using Nancy.ModelBinding;
    using Nancy.Tests;

    using Xunit;

    public class CaseSensitivityFixture
    {
        private Browser browser;

        public class MainModule : NancyModule
        {
            public MainModule()
            {
                Get["/"] = _ =>
                {
                    string name = this.Request.Query.animal.HasValue ? this.Request.Query.animal : "";
                    return name;
                };

                Get["/{ANIMAL}"] = args =>
                {
                    string name = args.animal.HasValue ? args.animal : "";
                    return name;
                };

                Get["/animal"] = _ =>
                {
                    Animal animal = this.Bind<Animal>();
                    return (animal.Type == null) ? HttpStatusCode.NoContent : HttpStatusCode.Accepted;
                };
            }
        }

        public class Animal
        {
            public string Type { get; set; }
        }

        public CaseSensitivityFixture()
        {
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module<MainModule>();
            });

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_pull_query_parameter_with_different_case()
        {
            StaticConfiguration.CaseSensitive = false;
            string animal = "dog";
            var response = browser.Get("/", with =>
            {
                with.Query("ANIMAL", animal);
            });

            response.Body.AsString().ShouldEqual(animal);
        }

        [Fact]
        public void Should_not_pull_query_parameter_with_different_case_when_sensitivity_is_on()
        {
            StaticConfiguration.CaseSensitive = true;
            string animal = "dog";
            var response = browser.Get("/", with =>
            {
                with.Query("ANIMAL", animal);
            });

            response.Body.AsString().ShouldEqual("");
        }

        [Fact]
        public void Should_pull_parameter_with_different_case()
        {
            StaticConfiguration.CaseSensitive = false;
            string animal = "dog";
            var response = browser.Get("/dog", with =>
            {
            });

            response.Body.AsString().ShouldEqual(animal);
        }

        [Fact]
        public void Should_not_pull_parameter_with_different_case_when_sensitivity_is_on()
        {
            StaticConfiguration.CaseSensitive = true;
            
            var response = browser.Get("/dog", with =>
            {
            });

            response.Body.AsString().ShouldEqual("");
        }

        [Fact]
        public void Should_bind_with_different_case()
        {
            StaticConfiguration.CaseSensitive = false;
            string animal = "dog";
            var response = browser.Get("/animal", with =>
            {
                with.Query("TYPE", animal);
            });

            response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
        }

        [Fact]
        public void Should_not_bind_with_different_case_when_sensitivity_is_on()
        {
            StaticConfiguration.CaseSensitive = true;
            string animal = "dog";
            var response = browser.Get("/animal", with =>
            {
                with.Query("TYPE", animal);
            });

            response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }


    }
}
