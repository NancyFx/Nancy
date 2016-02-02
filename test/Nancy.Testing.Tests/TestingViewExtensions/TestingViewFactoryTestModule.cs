namespace Nancy.Testing.Tests.TestingViewExtensions
{
    public class TestingViewFactoryTestModule : LegacyNancyModule
    {
        private const string VIEW_PATH = "TestingViewExtensions/ViewFactoryTest.sshtml";

        public TestingViewFactoryTestModule()
        {
            this.Get["/testingViewFactoryNoModel"] = _ => this.View[VIEW_PATH];
            this.Get["/testingViewFactory"] = _ => this.View[VIEW_PATH, GetModel()];
            this.Get["/testingViewFactoryNoViewName"] = _ => { return GetModel(); };
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