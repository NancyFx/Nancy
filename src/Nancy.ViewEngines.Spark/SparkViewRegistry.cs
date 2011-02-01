namespace Nancy.ViewEngines.Spark
{
    public class SparkViewRegistry : IViewEngineRegistry
    {
        private readonly ViewFactory viewFactory;

        public SparkViewRegistry()
        {
            viewFactory = new ViewFactory();
        }

        public string Extension
        {
            get { return ".spark"; }
        }

        public IViewEngine ViewEngine
        {
            get { return viewFactory; }
        }
    }
}
