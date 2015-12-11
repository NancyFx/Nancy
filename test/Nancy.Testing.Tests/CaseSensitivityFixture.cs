namespace Nancy.Testing.Tests
{
    using Nancy.ModelBinding;
    using Nancy.Tests;
    using Xunit;

    public class CaseSensitivityFixture
    {
        private readonly Browser browser;

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
            // Given
            const string animal = "dog";

            using (new StaticConfigurationContext(x => x.CaseSensitive = false))
            {
                // When
                var response = browser.Get("/", with =>
                {
                    with.Query("ANIMAL", animal);
                });

                // Then
                response.Body.AsString().ShouldEqual(animal);
            }
        }

        [Fact]
        public void Should_not_pull_query_parameter_with_different_case_when_sensitivity_is_on()
        {
            // Given
            const string animal = "dog";

            using (new StaticConfigurationContext(x => x.CaseSensitive = true))
            {

                // When
                var response = browser.Get("/", with =>
                {
                    with.Query("ANIMAL", animal);
                });

                // Then
                response.Body.AsString().ShouldEqual(string.Empty);
            }
        }

        [Fact]
        public void Should_pull_parameter_with_different_case()
        {
            // Given
            const string animal = "dog";

            using (new StaticConfigurationContext(x => x.CaseSensitive = false))
            {
                // When
                var response = browser.Get("/dog", with =>
                {
                });

                // Then
                response.Body.AsString().ShouldEqual(animal);
            }
        }

        [Fact]
        public void Should_not_pull_parameter_with_different_case_when_sensitivity_is_on()
        {
            // Given
            using (new StaticConfigurationContext(x => x.CaseSensitive = true))
            {
                // When
                var response = browser.Get("/dog");

                // Then
                response.Body.AsString().ShouldEqual(string.Empty);
            }
        }

        [Fact]
        public void Should_bind_query_with_different_case_when_sensitivity_is_off()
        {
            // Given
            const string animal = "dog";

            using (new StaticConfigurationContext(x => x.CaseSensitive = false))
            {
                // When
                var response = browser.Get("/animal", with =>
                {
                    with.Query("TYPE", animal);
                });

                // Then
                response.StatusCode.ShouldEqual(HttpStatusCode.Accepted);
            }
        }

        [Fact]
        public void Should_not_bind_query_with_different_case_when_sensitivity_is_on()
        {
            // Given
            const string animal = "dog";

            using (new StaticConfigurationContext(x => x.CaseSensitive = true))
            {
                // When
                var response = browser.Get("/animal", with =>
                {
                    with.Query("TYPE", animal);
                });

                // Then
                response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
            }
        }

        public class MainModule : NancyModule
        {
            public MainModule()
            {
                Get["/"] = _ =>
                {
                    var name = this.Request.Query.animal.HasValue ? this.Request.Query.animal : "";
                    return name;
                };

                Get["/{ANIMAL}"] = args =>
                {
                    var name = args.animal.HasValue ? args.animal : "";
                    return name;
                };

                Get["/animal"] = _ =>
                {
                    var animal = this.Bind<Animal>();
                    return (animal.Type == null) ? HttpStatusCode.NoContent : HttpStatusCode.Accepted;
                };
            }
        }

        public class Animal
        {
            public string Type { get; set; }
        }
    }
}
