namespace Nancy.Tests.Functional.Tests
{
    using System.Collections.Generic;

    using Nancy.Bootstrapper;
    using Nancy.ModelBinding;
    using Nancy.Testing;

    using Xunit;

    public class ModelBindingFixture
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly Browser browser;

        public ModelBindingFixture()
        {
            this.bootstrapper = 
                new ConfigurableBootstrapper(with => with.Modules(new[] { typeof(ModelBindingModule), typeof(MixedSourceModelBindingModule) }));

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_be_able_to_modelbind_json_content_to_list()
        {
            // Given
            const string body = "[{ 'key1': 'value1' , 'key2': 'value2'},{ 'key1': 'value1' , 'key2': 'value2'}, { 'key1': 'value1' , 'key2': 'value2'}]";

            // When
            var result = this.browser.Post("/jsonlist", with => with.Body(body, "application/json"));

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Should_be_able_to_bind_body_and_other_sources_separately()
        {
            // Given
            const string body = "{ 'key1': 'body' , 'key2': 'value2'}";

            // When
            var result = this.browser.Put("/foo/param", with => with.Body(body, "application/json"));

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }

    public class ModelBindingModule : NancyModule
    {
        public ModelBindingModule()
        {
            Post["/jsonlist"] = _ =>
            {
                var model = this.Bind<List<MyModel>>();

                return (model.Count == 3) ? 
                    HttpStatusCode.OK : 
                    HttpStatusCode.InternalServerError;
            };
        }
    }

    public class MixedSourceModelBindingModule : NancyModule
    {
        public MixedSourceModelBindingModule()
        {
            Put["/foo/{key1}"] = _ =>
            {
                var bodyModel = this.Bind<MyModel>(new BindingConfig { BodyOnly = true });
                var paramModel = this.Bind<ParamModel>();

                return (bodyModel.key1 != paramModel.key1) ?
                    HttpStatusCode.OK :
                    HttpStatusCode.InternalServerError;
            };
        }
    }

    public class ParamModel
    {
        public string key1 { get; set; }
    }

    public class MyModel
    {
        public string key1 { get; set; }
        public string key2 { get; set; }
    }
}