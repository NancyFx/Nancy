namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewRegistry : IViewEngineRegistry
    {
        //TODO - should not return a new ViewEngine every time.
        public IViewEngine ViewEngine
        {
            get { return new NDjangoViewEngine(); }
        }

        public string Extension
        {
            get { return ".django"; }
        }
    }
}