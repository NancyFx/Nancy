namespace Nancy.Testing.Tests.TestingViewExtensions
{
    using Xunit;

    public class GetModelExtententionsTests
    {
        private readonly Browser _browser;

        public GetModelExtententionsTests()
        {
            this._browser = new Browser(with =>
            {
                with.Module<TestingViewFactoryTestModule>();
                with.ViewFactory<TestingViewFactory>();
            });

        }
        [Fact]
        public void should_return_null_when_model_is_not_set()
        {
            var response = this._browser.Get("/testingViewFactoryNoModel");
            Assert.Null(response.GetModel<ViewFactoryTestModel>());
        }
        
        [Fact]
        public void should_not_return_null_when_model_is_set()
        {
            var response = this._browser.Get("/testingViewFactory");
            Assert.NotNull(response.GetModel<ViewFactoryTestModel>());
        }
        
        [Fact]
        public void should_return_model_of_correct_type()
        {
            var response = this._browser.Get("/testingViewFactory");            
            Assert.IsType<ViewFactoryTestModel>(response.GetModel<ViewFactoryTestModel>());
        }

        [Fact]
        public void should_set_values_correct_on_the_model()
        {
            var response = this._browser.Get("/testingViewFactory");
            var model = response.GetModel<ViewFactoryTestModel>();
            Assert.Equal("A value", model.AString);
        }

        [Fact]
        public void should_set_values_correct_on_a_complex_model()
        {
            var response = this._browser.Get("/testingViewFactory");
            var model = response.GetModel<ViewFactoryTestModel>();
            Assert.Equal("Another value", model.ComplexModel.AnotherString);
        }
    }
}
