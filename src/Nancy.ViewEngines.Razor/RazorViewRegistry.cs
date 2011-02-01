namespace Nancy.ViewEngines.Razor
{
    public class RazorViewRegistry : IViewEngineRegistry
    {
        public IViewEngine ViewEngine
        {
            get { return new RazorViewEngine(); }
        }

        public string Extension
        {
            get { return ".cshtml"; }
        }
    }
}