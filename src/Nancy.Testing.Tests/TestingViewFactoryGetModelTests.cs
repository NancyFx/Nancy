namespace Nancy.Testing.Tests
{
    using Xunit;

    public class TestingViewFactoryGetModelTests
    {
        private readonly BrowserResponse _response;
        private readonly Browser _browser;

        public TestingViewFactoryGetModelTests()
        {
            this._browser = new Browser(with =>
            {
                with.Module<TestingViewFactoryTestModule>();
                with.ViewFactory<TestingViewFactory>();
            });

            this._response = this._browser.Get("/testingViewFactory");
        }

        [Fact]
        public void should_return_model_of_correct_type()
        {
            Assert.IsType<ViewFactoryTestModel>(this._response.GetModel<ViewFactoryTestModel>());
        }

        [Fact]
        public void should_set_model()
        {
            Assert.NotNull(this._response.GetModel<ViewFactoryTestModel>());
        }

        [Fact]
        public void should_set_values_correct_on_the_model()
        {
            var model = this._response.GetModel<ViewFactoryTestModel>();
            Assert.Equal("A value", model.AString);
            Assert.Equal("Another value", model.ComplexModel.AnotherString);
        }

        [Fact]
        public void should_set_the_view_name()
        {
            Assert.Equal("ViewFactoryTest.sshtml", _response.GetViewName());
        }

        [Fact]
        public void should_set_the_view_name_to_empty_when_no_view_name_is_given()
        {
            var response = this._browser.Get("/testingViewFactoryNoViewName");
            Assert.Equal(string.Empty, response.GetViewName());
        }

        [Fact]
        public void should_set_the_module_name()
        {
            Assert.Equal("TestingViewFactoryTest", _response.GetModuleName());
        }

    }

    // Test module for TestingViewFactory

    public class TestingViewFactoryTestModule : NancyModule
    {
        public TestingViewFactoryTestModule()
        {
            Get["/testingViewFactory"] = _ => this.View["ViewFactoryTest.sshtml", GetModel()];
            Get["/testingViewFactoryNoViewName"] = _ => this.View[GetModel()];
        }

        private static ViewFactoryTestModel GetModel()
        {
            return new ViewFactoryTestModel
            {
                AString = "A value",
                ComplexModel = new CompositeTestModel { AnotherString = "Another value" }
            };
        }
    }
    
    public class ViewFactoryTestModel
    {
        public string AString { get; set; }
        public CompositeTestModel ComplexModel { get; set; }
    }

    public class CompositeTestModel
    {
        public string AnotherString { get; set; }
    }
}
