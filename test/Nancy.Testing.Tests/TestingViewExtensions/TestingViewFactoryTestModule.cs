namespace Nancy.Testing.Tests.TestingViewExtensions
{
    public class TestingViewFactoryTestModule : NancyModule
    {
        private const string VIEW_PATH = "TestingViewExtensions/ViewFactoryTest.sshtml";

        public TestingViewFactoryTestModule()
        {
            Get("/testingViewFactoryNoModel", args => this.View[VIEW_PATH]);
            Get("/testingViewFactory", args => this.View[VIEW_PATH, GetModel()]);
            Get("/testingViewFactoryNoViewName", args => { return GetModel(); });
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