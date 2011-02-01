namespace Nancy.ViewEngines.Spark
{
    public class SparkViewRegistry : IViewEngineRegistry
    {
        public string Extension
        {
            get { return ".spark"; }
        }

        public IViewEngine ViewEngine
        {
            get { return new ViewFactory(); }
        }
    }
}